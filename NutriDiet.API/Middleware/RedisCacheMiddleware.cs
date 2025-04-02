using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

public class RedisCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<RedisCacheMiddleware> _logger;

    // Danh sách paths được cache
    private static readonly string[] IncludedPaths = new[] { "/api/allergy", "/api/cuisine-type", "/api/disease", "/api/food", "/api/ingredient", "/api/package" };

    // Thời gian sống mặc định của cache là 15 phút
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(15);

    public RedisCacheMiddleware(
        RequestDelegate next,
        IConnectionMultiplexer? redis,
        ILogger<RedisCacheMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _redisDatabase = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Chỉ xử lý các yêu cầu GET
        if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        string path = context.Request.Path.Value.ToLowerInvariant();
        // Chỉ cache nếu path nằm trong IncludedPaths (so sánh chính xác, không dùng StartsWith)
        if (!IncludedPaths.Contains(path))
        {
            _logger.LogDebug("Skipping cache for path: {Path}", path);
            await _next(context);
            return;
        }

        string cacheKey = GenerateCacheKey(context);
        try
        {
            var cachedResponse = await _redisDatabase.StringGetAsync(cacheKey);
            if (cachedResponse.HasValue)
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(cachedResponse.ToString());
                return;
            }
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable for {CacheKey}. Proceeding without cache.", cacheKey);
            // Tiếp tục pipeline nếu Redis lỗi
        }

        var originalBodyStream = context.Response.Body;
        using var newBodyStream = new MemoryStream();
        try
        {
            context.Response.Body = newBodyStream;
            await _next(context);

            // Sao chép header từ phản hồi gốc để bảo toàn CORS
            var headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value);

            newBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

            // Lưu vào cache nếu phản hồi hợp lệ
            if (context.Response.StatusCode == StatusCodes.Status200OK &&
                context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true &&
                !string.IsNullOrEmpty(responseBody) &&
                responseBody.Length < 1024 * 1024) // Giới hạn 1MB
            {
                try
                {
                    await _redisDatabase.StringSetAsync(cacheKey, responseBody, DefaultExpiration);
                    _logger.LogInformation("Cached {CacheKey}", cacheKey);
                }
                catch (RedisException ex)
                {
                    _logger.LogError(ex, "Failed to cache response for {CacheKey}", cacheKey);
                }
            }

            // Khôi phục body và header
            context.Response.Body = originalBodyStream;
            foreach (var header in headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }
            await context.Response.WriteAsync(responseBody);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private string GenerateCacheKey(HttpContext context)
    {
        string path = context.Request.Path.Value.ToLowerInvariant();
        string queryString = context.Request.QueryString.ToString();
        string version = "v1";
        string rawKey = $"api:shared:{version}:{path}{queryString}";
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rawKey)));
    }
}
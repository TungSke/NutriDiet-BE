using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class RedisCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<RedisCacheMiddleware> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Danh sách paths được cache.
    private static readonly string[] IncludedPaths = new[] { "/api/allergy", "/api/cuisine-type", "/api/disease", "/api/food", "/api/ingredient" , "/api/package" };

    // Danh sách paths bị loại trừ khỏi cache, như /api/user/whoami.
    private static readonly string[] ExcludedPaths = new[] { "/api/user/whoami" };

    // Danh sách paths cần cache riêng theo người dùng, như /api/meal-plan.
    private static readonly string[] PersonalizedPaths = new[] { "/api/meal-plan" };

    // Thời gian sống mặc định của cache là 15 phút.
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(15);

    public RedisCacheMiddleware(
        RequestDelegate next,
        IConnectionMultiplexer? redis,
        ILogger<RedisCacheMiddleware> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _redisDatabase = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        string path = context.Request.Path.Value.ToLowerInvariant();

        // Kiểm tra IncludedPaths
        if (!IncludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogDebug("Skipping cache for non-included path: {Path}", path);
            await _next(context);
            return;
        }

        // Kiểm tra ExcludedPaths
        if (ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogDebug("Skipping cache for excluded path: {Path}", path);
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
                await context.Response.WriteAsync(cachedResponse.ToString());
                return;
            }
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable for {CacheKey}. Proceeding without cache.", cacheKey);
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var newBodyStream = new MemoryStream();
        context.Response.Body = newBodyStream;

        await _next(context);

        newBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

        if (context.Response.StatusCode == StatusCodes.Status200OK &&
            context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true &&
            !string.IsNullOrEmpty(responseBody))
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

        context.Response.Body = originalBodyStream;
        await context.Response.WriteAsync(responseBody);
    }

    //cache riêng biệt từng user
    private string GenerateCacheKey(HttpContext context)
    {
        string path = context.Request.Path.Value.ToLowerInvariant();
        string queryString = context.Request.QueryString.ToString();
        string version = "v1";
        string userId = PersonalizedPaths.Any(p => path.StartsWith(p))
            ? (context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous")
            : "shared";
        string rawKey = $"api:shared:{userId}:{version}:{path}{queryString}";
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rawKey)));
    }
}
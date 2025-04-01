using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.API.Middleware
{
    public class RedisCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache? _cache; // Có thể null
        private readonly ILogger<RedisCacheMiddleware> _logger;
        private readonly RedisCacheOptions _options;

        public RedisCacheMiddleware(
            RequestDelegate next,
            IDistributedCache cache, // Không ném lỗi nếu null, sẽ kiểm tra sau
            ILogger<RedisCacheMiddleware> logger,
            RedisCacheOptions options = null)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cache = cache; // Không kiểm tra null ở đây
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? new RedisCacheOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                await _next(context);
                return;
            }

            // Nếu Redis không được cấu hình, bỏ qua cache
            if (_cache == null)
            {
                _logger.LogDebug("Redis cache is not configured. Skipping cache processing.");
                await _next(context);
                return;
            }

            string cacheKey = GenerateCacheKey(context);

            if (_options.ExcludedPaths != null && _options.ExcludedPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug("Skipping cache for excluded path: {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            string cachedResponse = await GetCachedResponseAsync(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogInformation("Cache hit for {CacheKey} in {ElapsedMs}ms", cacheKey, stopwatch.ElapsedMilliseconds);
                await WriteResponseAsync(context, cachedResponse);
                return;
            }

            await ProcessAndCacheResponseAsync(context, cacheKey, stopwatch);
        }

        private async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            try
            {
                return await _cache.GetStringAsync(cacheKey, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable for {CacheKey}. Proceeding without cache.", cacheKey);
                return null;
            }
        }

        private async Task ProcessAndCacheResponseAsync(HttpContext context, string cacheKey, Stopwatch stopwatch)
        {
            var originalBodyStream = context.Response.Body;
            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                responseBody.Seek(0, SeekOrigin.Begin);
                string responseContent = await new StreamReader(responseBody, Encoding.UTF8).ReadToEndAsync();

                if (context.Response.StatusCode == StatusCodes.Status200OK &&
                    context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true &&
                    !string.IsNullOrEmpty(responseContent))
                {
                    stopwatch.Restart();
                    await CacheResponseAsync(cacheKey, responseContent);
                    _logger.LogInformation("Cached {CacheKey} in {ElapsedMs}ms", cacheKey, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogDebug("Skipping cache for {CacheKey} due to non-200 status, non-JSON content, or empty response.", cacheKey);
                }

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing response for {CacheKey}", cacheKey);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task CacheResponseAsync(string cacheKey, string responseContent)
        {
            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _options.DefaultExpiration,
                    SlidingExpiration = _options.SlidingExpiration
                };
                await _cache.SetStringAsync(cacheKey, responseContent, cacheOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache response for {CacheKey}", cacheKey);
            }
        }

        private Task WriteResponseAsync(HttpContext context, string content)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            return context.Response.WriteAsync(content);
        }

        private string GenerateCacheKey(HttpContext context)
        {
            string rawKey = $"api:{context.Request.Path}{context.Request.QueryString}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public class RedisCacheOptions
    {
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(24);
        public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public string[] ExcludedPaths { get; set; } = Array.Empty<string>();
    }
}
//namespace NutriDiet.API.Middleware
//{
//    public class RedisCacheMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IDistributedCache _cache;
//        private readonly ILogger<RedisCacheMiddleware> _logger;
//        private readonly RedisCacheOptions _options;

//        public RedisCacheMiddleware(
//            RequestDelegate next,
//            IDistributedCache cache,
//            ILogger<RedisCacheMiddleware> logger,
//            RedisCacheOptions options = null)
//        {
//            _next = next ?? throw new ArgumentNullException(nameof(next));
//            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _options = options ?? new RedisCacheOptions();
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            // Chỉ xử lý request GET
//            if (context.Request.Method != HttpMethods.Get)
//            {
//                await _next(context);
//                return;
//            }

//            string cacheKey = GenerateCacheKey(context);

//            // Kiểm tra nếu endpoint bị loại trừ
//            if (_options.ExcludedPaths != null && _options.ExcludedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
//            {
//                _logger.LogDebug("Skipping cache for excluded path: {Path}", context.Request.Path);
//                await _next(context);
//                return;
//            }

//            // Kiểm tra cache
//            var stopwatch = Stopwatch.StartNew();
//            string cachedResponse = await GetCachedResponseAsync(cacheKey);
//            if (cachedResponse != null)
//            {
//                _logger.LogInformation("Cache hit for {CacheKey} in {ElapsedMs}ms", cacheKey, stopwatch.ElapsedMilliseconds);
//                await WriteResponseAsync(context, cachedResponse);
//                return;
//            }

//            // Nếu không có cache, gọi API và lưu response
//            await ProcessAndCacheResponseAsync(context, cacheKey, stopwatch);
//        }

//        private async Task<string> GetCachedResponseAsync(string cacheKey)
//        {
//            try
//            {
//                return await _cache.GetStringAsync(cacheKey);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogWarning("Redis unavailable: {Message}. Proceeding without cache.", ex.Message);
//                return null; // Fallback khi Redis lỗi
//            }
//        }

//        private async Task ProcessAndCacheResponseAsync(HttpContext context, string cacheKey, Stopwatch stopwatch)
//        {
//            var originalBodyStream = context.Response.Body;
//            try
//            {
//                using var responseBody = new MemoryStream();
//                context.Response.Body = responseBody;

//                await _next(context);

//                responseBody.Seek(0, SeekOrigin.Begin);
//                string responseContent = await new StreamReader(responseBody, Encoding.UTF8).ReadToEndAsync();

//                if (context.Response.StatusCode == StatusCodes.Status200OK)
//                {
//                    stopwatch.Restart();
//                    await CacheResponseAsync(cacheKey, responseContent);
//                    _logger.LogInformation("Cached {CacheKey} in {ElapsedMs}ms", cacheKey, stopwatch.ElapsedMilliseconds);
//                }

//                responseBody.Seek(0, SeekOrigin.Begin);
//                await responseBody.CopyToAsync(originalBodyStream);
//            }
//            finally
//            {
//                context.Response.Body = originalBodyStream; // Đảm bảo khôi phục stream gốc
//            }
//        }

//        private async Task CacheResponseAsync(string cacheKey, string responseContent)
//        {
//            try
//            {
//                var cacheOptions = new DistributedCacheEntryOptions
//                {
//                    AbsoluteExpirationRelativeToNow = _options.DefaultExpiration,
//                    SlidingExpiration = _options.SlidingExpiration
//                };
//                await _cache.SetStringAsync(cacheKey, responseContent, cacheOptions);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError("Failed to cache response for {CacheKey}: {Message}", cacheKey, ex.Message);
//            }
//        }

//        private async Task WriteResponseAsync(HttpContext context, string content)
//        {
//            context.Response.ContentType = "application/json";
//            await context.Response.WriteAsync(content);
//        }

//        private string GenerateCacheKey(HttpContext context)
//        {
//            // Tạo key ngắn gọn và duy nhất
//            string path = context.Request.Path.Value.ToLowerInvariant();
//            string query = context.Request.QueryString.Value ?? "";
//            return $"api:{path}{query}".GetHashCode().ToString(); // Hash để tối ưu độ dài key
//        }
//    }

//    // Class tùy chọn cấu hình middleware
//    public class RedisCacheOptions
//    {
//        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(24);
//        public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);
//        public string[] ExcludedPaths { get; set; } = Array.Empty<string>();
//    }
//}

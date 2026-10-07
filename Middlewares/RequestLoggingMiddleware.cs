using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookManagerApp.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "";

            // 1. Ghi log request
            Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

            // 2. Kiểm tra chặn ID không hợp lệ (0 hoặc số âm)
            if (path.EndsWith("/0") || path.EndsWith("/-1") ||
                path.Contains("/Details/0") || path.Contains("/Details/-1") ||
                path.Contains("/Detail/0") || path.Contains("/Detail/-1"))
            {
                context.Response.StatusCode = 400; // Trả về mã lỗi 400
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Book id không hợp lệ");
                return; // Ngắt luồng ngay lập tức, không đi tiếp
            }

            await _next(context);

            stopwatch.Stop();
            Console.WriteLine($"Status Code: {context.Response.StatusCode} (thoi gian xu lycode : {stopwatch.ElapsedMilliseconds} ms)");
        }
    }
}
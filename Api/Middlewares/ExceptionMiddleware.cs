using Api.Errors;
using System;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment env;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next  ,IHostEnvironment env , ILogger<ExceptionMiddleware> logger)
        {
            this.next=next;
            this.env=env;
            this.logger=logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json"; 
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var ResponseMessage = env.IsDevelopment()
                    ? new ServerErrorResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ServerErrorResponse((int)HttpStatusCode.InternalServerError , ex.Message , "Internal server error");

                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(ResponseMessage, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}

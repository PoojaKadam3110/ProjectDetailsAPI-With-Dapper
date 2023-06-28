using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using System.Net;

namespace Application.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment Env;
        public ExceptionMiddleware(RequestDelegate next, 
                                    ILogger<ExceptionMiddleware> logger,
                                     IHostEnvironment env)
        {
            Env = env;
            Next = next;
            _logger = logger;   
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch(Exception ex) 
            {
                ApiError response;
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                String message;
                var exceptionType = ex.GetType();   

                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = "You are not authorized";
                }else 
                { 
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Some unknown error occurred";
                }

                if (Env.IsDevelopment())
                {
                    response = new ApiError((int)statusCode,ex.Message,ex.StackTrace.ToString());
                }
                else
                {
                    response = new ApiError((int)statusCode, message);
                }
                _logger.LogError(ex,ex.Message);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
        }
    }
}

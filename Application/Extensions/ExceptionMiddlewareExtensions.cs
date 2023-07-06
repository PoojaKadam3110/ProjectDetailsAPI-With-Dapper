using Application.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using Swashbuckle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app,IHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>(); //for error
        }
        public static void ConfigureBuiltinExceptionHandler(this IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseExceptionHandler(options =>
                //{
                //    options.Run(
                //        async context =>
                //        {
                //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //            var ex = context.Features.Get<IExceptionHandlerFeature>();
                //            if (ex != null)
                //            {
                //                await context.Response.WriteAsync(ex.Error.Message);
                //            }
                //        }
                //        );
                //}
                //);
            }           
        }
    }
}

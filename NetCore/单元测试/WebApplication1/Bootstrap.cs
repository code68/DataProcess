using System;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace WebApplication1
{
    public static class Bootstrap
    {
        /// <summary>
        /// AutoMapper的全局引用
        /// </summary>
        public static IMapper Mapper { get;  private set; }
        public static IServiceCollection Service { get;  private set; }

        public static void ConfigurationServices(IServiceCollection services, IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            services.AddMemoryCache();

            // services.AddSwaggerGen(options =>
            // {
            //     options.SwaggerDoc("v1", new OpenApiInfo() { Title = "在线面试系统 API", Version = "v1"});
            // });
            
            #region 手动注入容器

            //services.AddScoped<Repository, RepositoryImpl>();
            //// 添加HttpContextAccessor
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //// 添加automapper
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            #endregion

            Service = services;
        }

        public static void ConfigurationApp(IApplicationBuilder app, IServiceProvider provider, IConfiguration configuration)
        {
            Mapper = provider.GetService<IMapper>();
            // app.UseGlobalApiLoggingMiddleware();
           
            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "在线面试系统");
            //});
        }
    }
}

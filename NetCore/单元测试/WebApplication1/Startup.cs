using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string Default_Cors = "All";


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

     

            services.AddCors(options =>
            {
                options.AddPolicy(Default_Cors, builder =>
                {
                    builder.SetIsOriginAllowed(x => true).AllowCredentials();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });


            services.AddControllers();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddScoped<UserService>();

            if (services.All(u => u.ServiceType != typeof(IHttpContextAccessor)))
            {
                services.AddScoped(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));
            }


            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            Bootstrap.ConfigurationServices(services, Configuration);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider, IConfiguration configuration)
        {
            app.UseHsts();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //loggerFactory.AddProvider(new SignalRLoggerProvider());

            #region 加大线程池

            ThreadPool.SetMinThreads(50, 50);
            ThreadPool.SetMaxThreads(4000, 4000);

            #endregion


            app.UseCors(Default_Cors);

            app.UseRouting();

            app.UseAuthorization();
      

        }
    }
}

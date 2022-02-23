using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;
using System.Net.Http;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsProduction())
            {
                Console.WriteLine("--> Using SQL Server DB");
                services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));
            }
            else 
            {
                Console.WriteLine("--> Using InMem DB");
                services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
            }
            
            // Services can be registered with one of the following lifetimes:
            // (1) Transient: created each time they're requested from the service container, works best for lightweight, stateless services
            // (2) Scoped: created once per client request (connection)
            // (3) Singleton: every subsequent request of the service implementation from the dependency injection container uses the same instance.
            services.AddScoped<IPlatformRepo, PlatformRepo>();

            // AutoMapper: maps an object of one type to another type
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var httpClientBuilder = services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            /*
            // need to add the following setting to bypass the certificate !!!!!!!!!!!
            // otherwise, get error: the SSL connection could not be established
            httpClientBuilder.ConfigureHttpMessageHandlerBuilder(builder => 
            {
                var clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                builder.PrimaryHandler = clientHandler;
                builder.Build();
            });*/

            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddGrpc();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            // get rid of the warning: failed to determine the https port for redirect
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();
                endpoints.MapGet("/protos/platforms.proto", async context => 
                {
                    await context.Response.WriteAsync(File.ReadAllText("/protos/platforms.proto"));
                });
            });

            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}

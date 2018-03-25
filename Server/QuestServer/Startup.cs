﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Utils.Networking;

namespace QuestServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.MapWebSocketManager("/quest", serviceProvider.GetService<QuestMessageHandler>());

            app.UseMvc();
        }
    }

    public static class WebSocketExtensions {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                      PathString path,
                                      WebSocketHandler handler) {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services) {
            services.AddTransient<WebSocketConnectionManager>();
            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes) {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler)) {
                    services.AddSingleton(type);
                }
            }
            return services;
        }
    }
}
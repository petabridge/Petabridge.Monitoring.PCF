// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Akka.Actor;
using Akka.Bootstrap.PCF;
using Akka.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Petabridge.Cmd.Host;

namespace Petabridge.Monitoring.PCF.Demo
{
    public class Startup
    {
        public static ActorSystem system;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            system = ActorSystem.Create("pb", ConfigurationFactory.ParseString(@"akka.loglevel = DEBUG"));
            var cmd = PetabridgeCmd.Get(system);
            cmd.Start();

            var metrics = PcfMetricRecorder.Create(system);

            app.Run(async context =>
            {
                var start = metrics.TimeProvider.NowUnixEpoch;
                metrics.IncrementCounter("http.serv");
                var environment = PcfEnvironment.Instance.Value.ToString() + Environment.NewLine +
                                  Environment.GetEnvironmentVariable("VCAP_SERVICES");
                await context.Response.WriteAsync(environment);
                metrics.RecordTiming("http.serv", metrics.TimeProvider.NowUnixEpoch - start);
            });
        }
    }
}
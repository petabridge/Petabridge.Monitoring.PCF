// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Bootstrap.PCF;
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

            system = ActorSystem.Create("pb");
            var cmd = PetabridgeCmd.Get(system);
            cmd.Start();

            var metrics = PcfMetricRecorder.Create();

            app.Run(async context =>
            {
                var start = metrics.TimeProvider.NowUnixEpoch;
                metrics.IncrementCounter("http.serv");
                await context.Response.WriteAsync(PcfEnvironment.Instance.Value.ToString());
                metrics.RecordTiming("http.serv", metrics.TimeProvider.NowUnixEpoch - start);
            });
        }
    }
}
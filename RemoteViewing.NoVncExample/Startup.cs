using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemoteViewing.AspNetCore;
using RemoteViewing.Logging;

namespace RemoteViewing.NoVncExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseVncServer(
                "/novnc",
                (context) => new VncContext()
                {
                    Password = "demo",
                    FramebufferSource = new DummyFramebufferSource()
                });

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}

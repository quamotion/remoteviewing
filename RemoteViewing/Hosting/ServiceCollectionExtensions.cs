using Microsoft.Extensions.DependencyInjection;
using RemoteViewing.Vnc.Server;

namespace RemoteViewing.Hosting
{
    /// <summary>
    /// Extension methods for setting up VNC services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a VNC server to the service collection.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the VNC server to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddVncServer(this IServiceCollection services)
            => AddVncServer<VncServer>(services);

        /// <summary>
        /// Adds a VNC server to the service collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of a class implementing a VNC server.
        /// </typeparam>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the VNC server to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddVncServer<T>(this IServiceCollection services)
            where T : class, IVncServer
            => AddVncServer<T>(services, new VncServerOptions());

        /// <summary>
        /// Adds a VNC server to the service collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of a class implementing a VNC server.
        /// </typeparam>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add the VNC server to.
        /// </param>
        /// <param name="options">
        /// Options which control how the <see cref="VncServer"/> is created.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddVncServer<T>(this IServiceCollection services, VncServerOptions options)
            where T : class, IVncServer
        {
            services.AddSingleton(options);
            services.AddSingleton<IVncServer, T>();
            services.AddHostedService<VncServerWorker>();
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Logging
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers a service interface and its implementation with a dynamic proxy and interceptor.
        /// </summary>
        /// <typeparam name="TService">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <param name="services">The IServiceCollection instance.</param>
        public static void AddProxiedService<TService, TImplementation>(
            this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService, new()
        {
            services.AddScoped<TService>(provider =>
            {
                var implementation = new TImplementation(); // Create the original implementation
                var interceptor = new MethodInterceptor();  // Create the interceptor instance
                return ProxyFactory.CreateProxy<TService>(implementation, interceptor); // Wrap with proxy
            });
        }

        /// <summary>
        /// Registers a dependent service interface and its implementation with a dynamic proxy and interceptor.
        /// </summary>
        /// <typeparam name="TService">The dependent service interface type.</typeparam>
        /// <typeparam name="TImplementation">The dependent service implementation type.</typeparam>
        /// <typeparam name="TDependency">The dependency service type.</typeparam>
        /// <param name="services">The IServiceCollection instance.</param>
        public static void AddProxiedServiceWithDependency<TService, TImplementation, TDependency>(
            this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            where TDependency : class
        {
            services.AddScoped<TService>(provider =>
            {
                var dependency = provider.GetRequiredService<TDependency>(); // Resolve the dependency
                var implementation = (TImplementation)Activator.CreateInstance(typeof(TImplementation), dependency); // Create the implementation with dependency
                var interceptor = new MethodInterceptor(); // Create the interceptor instance
                return ProxyFactory.CreateProxy<TService>(implementation, interceptor); // Wrap with proxy
            });
        }
    }
}

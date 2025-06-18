using Castle.DynamicProxy;

namespace Logging
{
    public static class ProxyFactory
    {
        private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public static T CreateProxy<T>(T target, IAsyncInterceptor interceptor) where T : class
        {
            return _proxyGenerator.CreateInterfaceProxyWithTarget(target, interceptor);
        }
    }
}


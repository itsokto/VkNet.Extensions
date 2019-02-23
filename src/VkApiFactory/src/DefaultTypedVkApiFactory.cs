using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <inheritdoc />
    internal class DefaultTypedVkApiFactory<TClient> : ITypedVkApiFactory<TClient>
    {
        private readonly Cache _cache;
        private readonly IServiceProvider _services;

        public DefaultTypedVkApiFactory(Cache cache, IServiceProvider services)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public TClient CreateVkApi(IVkApi vkApi)
        {
            if (vkApi == null)
            {
                throw new ArgumentNullException(nameof(vkApi));
            }
            return (TClient) _cache.Activator(_services, new object[] {vkApi});

        }

        // The Cache should be registered as a singleton, so it that it can
        // act as a cache for the Activator. This allows the outer class to be registered
        // as a transient, so that it doesn't close over the application root service provider.
        public class Cache
        {
            private static readonly Func<ObjectFactory> CreateActivator = () =>
                ActivatorUtilities.CreateFactory(typeof(TClient), new[] {typeof(IVkApi)});

            private ObjectFactory _activator;
            private bool _initialized;
            private object _lock;

            public ObjectFactory Activator => LazyInitializer.EnsureInitialized(
                ref _activator,
                ref _initialized,
                ref _lock,
                CreateActivator);
        }
    }
}
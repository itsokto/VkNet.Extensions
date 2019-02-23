using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    public class DefaultVkApiFactory : IVkApiFactory
    {
        private readonly IOptionsMonitor<VkApiFactoryOptions> _optionsMonitor;

        private readonly ConcurrentDictionary<string, IVkApi> _vkApis;

        public DefaultVkApiFactory(IOptionsMonitor<VkApiFactoryOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            _vkApis = new ConcurrentDictionary<string, IVkApi>();
        }

        public IVkApi CreateVkApi(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _vkApis.GetOrAdd(name, factory =>
            {
                var options = _optionsMonitor.Get(name);

                var api = new VkApi(options.Services);

                foreach (var action in options.VkApiActions)
                {
                    action(api);
                }

                return api;
            });
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace VkNet.VkApiFactory
{
    internal class DefaultVkApiBuilder : IVkApiBuilder
    {
        public DefaultVkApiBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public string Name { get; }
        public IServiceCollection Services { get; }
    }
}
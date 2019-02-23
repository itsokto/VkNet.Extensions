using Microsoft.Extensions.DependencyInjection;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <summary>
    /// A builder for configuring named <see cref="IVkApi"/> instances returned by <see cref="IVkApiFactory"/>.
    /// </summary>
    public interface IVkApiBuilder
    {
        /// <summary>
        /// Gets the name of the api configured by this builder.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
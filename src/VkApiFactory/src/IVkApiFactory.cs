using Microsoft.Extensions.DependencyInjection;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <summary>
    /// A factory abstraction for a component that can create <see cref="IVkApi"/> instances with custom
    /// configuration for a given logical name.
    /// </summary>
    /// <remarks>
    /// A default <see cref="IVkApiFactory"/> can be registered in an <see cref="IServiceCollection"/>
    /// by calling <see cref="VkApiFactoryServiceCollectionExtensions.AddVkApi(IServiceCollection)"/>.
    /// The default <see cref="IVkApiFactory"/> will be registered in the service collection as a singleton.
    /// </remarks>
    public interface IVkApiFactory
    {
        /// <summary>
        /// Creates and configures an <see cref="IVkApi"/> instance using the configuration that corresponds
        /// to the logical name specified by <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The logical name of the api to create.</param>
        /// <returns>A new <see cref="IVkApi"/> instance.</returns>
        /// <remarks>
        /// <para>
        /// Callers are also free to mutate the returned <see cref="IVkApi"/> instance's public properties
        /// as desired.
        /// </para>
        /// </remarks>
        IVkApi CreateVkApi(string name);
    }
}
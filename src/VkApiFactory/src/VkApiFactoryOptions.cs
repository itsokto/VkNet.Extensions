using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <summary>
    /// An options class for configuring the default <see cref="IVkApiFactory"/>.
    /// </summary>
    public class VkApiFactoryOptions
    {
        /// <summary>
        /// Gets a list of operations used to configure an <see cref="IVkApi"/>.
        /// </summary>
        public IList<Action<IVkApi>> VkApiActions { get; } = new List<Action<IVkApi>>();

        /// <summary>
        /// Gets the service collection to initialize an <see cref="IVkApi"/>.
        /// </summary>
        public IServiceCollection Services { get; set; } = new ServiceCollection();
    }
}
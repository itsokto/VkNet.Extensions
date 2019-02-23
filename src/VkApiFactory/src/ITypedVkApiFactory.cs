using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    public interface ITypedVkApiFactory<TClient>
    {
        /// <summary>
        /// Creates a typed client given an associated <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="vkApi">
        /// An <see cref="vkApi"/> created by the <see cref="IVkApiFactory"/> for the named client
        /// associated with <typeparamref name="TClient"/>.
        /// </param>
        /// <returns>An instance of <typeparamref name="TClient"/>.</returns>
        TClient CreateVkApi(IVkApi vkApi);
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <summary>
    /// Extension methods for configuring an <see cref="IVkApiBuilder"/>
    /// </summary>
    public static class VkApiBuilderExtensions
    {
        /// <summary>
        /// Adds a delegate that will be used to configure a named <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureApi">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        public static IVkApiBuilder ConfigureVkApi(this IVkApiBuilder builder, Action<IVkApi> configureApi)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureApi == null)
            {
                throw new ArgumentNullException(nameof(configureApi));
            }
            

            builder.Services.Configure<VkApiFactoryOptions>(builder.Name, options =>
            {
                options.VkApiActions.Add(configureApi);
                options.Services = builder.Services;
            });

            return builder;
        }
        
        /// <summary>
        /// Adds a delegate that will be used to configure a named <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureApi">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// The <see cref="IServiceProvider"/> provided to <paramref name="configureApi"/> will be the
        /// same application's root service provider instance.
        /// </remarks>
        public static IVkApiBuilder ConfigureVkApi(this IVkApiBuilder builder, Action<IServiceProvider, IVkApi> configureApi)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureApi == null)
            {
                throw new ArgumentNullException(nameof(configureApi));
            }

            builder.Services.AddTransient<IConfigureOptions<VkApiFactoryOptions>>(services =>
            {
                return new ConfigureNamedOptions<VkApiFactoryOptions>(builder.Name, options =>
                {
                    options.VkApiActions.Add(api => configureApi(services, api));
                    options.Services = builder.Services;
                });
            });

            return builder;
        }
        
                /// <summary>
        /// Configures a binding between the <typeparamref name="TClient" /> type and the named <see cref="IVkApi"/>
        /// associated with the <see cref="IVkApiBuilder"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="builder">The <see cref="IVkApiBuilder"/>.</param>
        /// <remarks>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Calling <see cref="VkApiBuilderExtensions.AddTypedVkApi{TClient}(IVkApiBuilder)"/> will register a typed
        /// client binding that creates <typeparamref name="TClient"/> using the <see cref="ITypedVkApiFactory{TClient}" />.
        /// </para>
        /// <para>
        /// The typed client's service dependencies will be resolved from the same service provider
        /// that is used to resolve the typed client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddTypedVkApi<TClient>(this IVkApiBuilder builder)
            where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient(s =>
            {
                var vkApiFactory = s.GetRequiredService<IVkApiFactory>();
                var vkApi = vkApiFactory.CreateVkApi(builder.Name);

                var typedClientFactory = s.GetRequiredService<ITypedVkApiFactory<TClient>>();
                return typedClientFactory.CreateVkApi(vkApi);
            });

            return builder;
        }

        /// <summary>
        /// Configures a binding between the <typeparamref name="TClient" /> type and the named <see cref="IVkApi"/>
        /// associated with the <see cref="IVkApiBuilder"/>. The created instances will be of type 
        /// <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The declared type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TImplementation}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. The type specified by will be instantiated by the 
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>.
        /// </typeparam>
        /// <param name="builder">The <see cref="IVkApiBuilder"/>.</param>
        /// <remarks>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Calling <see cref="VkApiBuilderExtensions.AddTypedVkApi{TClient,TImplementation}(IVkApiBuilder)"/>
        /// will register a typed client binding that creates <typeparamref name="TImplementation"/> using the 
        /// <see cref="ITypedVkApiFactory{TImplementation}" />.
        /// </para>
        /// <para>
        /// The typed client's service dependencies will be resolved from the same service provider
        /// that is used to resolve the typed client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddTypedVkApi<TClient, TImplementation>(this IVkApiBuilder builder)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<TClient>(s =>
            {
                var vkApiFactory = s.GetRequiredService<IVkApiFactory>();
                var vkApi = vkApiFactory.CreateVkApi(builder.Name);

                var typedClientFactory = s.GetRequiredService<ITypedVkApiFactory<TImplementation>>();
                return typedClientFactory.CreateVkApi(vkApi);
            });

            return builder;
        }

        /// <summary>
        /// Configures a binding between the <typeparamref name="TClient" /> type and the named <see cref="IVkApi"/>
        /// associated with the <see cref="IVkApiBuilder"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service.
        /// </typeparam>
        /// <param name="builder">The <see cref="IVkApiBuilder"/>.</param>
        /// <param name="factory">A factory function that will be used to construct the typed client.</param>
        /// <remarks>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Calling <see cref="VkApiBuilderExtensions.AddTypedVkApi{TClient}(IVkApiBuilder,Func{IVkApi,TClient})"/>
        /// will register a typed client binding that creates <typeparamref name="TClient"/> using the provided factory function.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddTypedVkApi<TClient>(this IVkApiBuilder builder, Func<IVkApi, TClient> factory)
            where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.Services.AddTransient(s =>
            {
                var vkApiFactory = s.GetRequiredService<IVkApiFactory>();
                var vkApi = vkApiFactory.CreateVkApi(builder.Name);

                return factory(vkApi);
            });

            return builder;
        }

        /// <summary>
        /// Configures a binding between the <typeparamref name="TClient" /> type and the named <see cref="IVkApi"/>
        /// associated with the <see cref="IVkApiBuilder"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service.
        /// </typeparam>
        /// <param name="builder">The <see cref="IVkApiBuilder"/>.</param>
        /// <param name="factory">A factory function that will be used to construct the typed client.</param>
        /// <remarks>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Calling <see cref="VkApiBuilderExtensions.AddTypedVkApi{TClient}(IVkApiBuilder,Func{IVkApi,IServiceProvider,TClient})"/>
        /// will register a typed client binding that creates <typeparamref name="TClient"/> using the provided factory function.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddTypedVkApi<TClient>(this IVkApiBuilder builder, Func<IVkApi, IServiceProvider, TClient> factory)
            where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.Services.AddTransient(s =>
            {
                var vkApiFactory = s.GetRequiredService<IVkApiFactory>();
                var vkApi = vkApiFactory.CreateVkApi(builder.Name);

                return factory(vkApi, s);
            });

            return builder;
        }
    }
}
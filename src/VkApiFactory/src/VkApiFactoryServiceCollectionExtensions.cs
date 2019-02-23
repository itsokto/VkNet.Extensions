using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;

namespace VkNet.VkApiFactory
{
    /// <summary>
    /// Extensions methods to configure an <see cref="IServiceCollection"/> for <see cref="IVkApiFactory"/>.
    /// </summary>
    public static class VkApiFactoryServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddVkApi(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            services.AddSingleton<DefaultVkApiFactory>();
            services.TryAddSingleton<IVkApiFactory>(serviceProvider =>
                serviceProvider.GetRequiredService<DefaultVkApiFactory>());

            //
            // Typed Clients
            //
            services.TryAddTransient(typeof(ITypedVkApiFactory<>),
                typeof(DefaultTypedVkApiFactory<>));
            services.TryAddTransient(typeof(DefaultTypedVkApiFactory<>.Cache),
                typeof(DefaultTypedVkApiFactory<>.Cache));

            return services;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the api.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi"/> and providing the matching name.
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi(this IServiceCollection services, string name)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddVkApi(services);

            return new DefaultVkApiBuilder(services, name);
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureApi">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the api.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi"/> and providing the matching name.
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default api.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi(this IServiceCollection services, string name,
            Action<IVkApi> configureApi)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureApi == null)
            {
                throw new ArgumentNullException(nameof(configureApi));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureApi);
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="IVkApi"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureApi">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the api.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi"/> and providing the matching name.
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default api.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi(this IServiceCollection services, string name,
            Action<IServiceProvider, IVkApi> configureApi)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureApi == null)
            {
                throw new ArgumentNullException(nameof(configureApi));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureApi);
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient"/> type and a named <see cref="IVkApi"/>. The client name
        /// will be set to the full name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient"/> type and a named <see cref="IVkApi"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services, string name)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services, string name)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services, Action<IVkApi> configureClient)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services,
            Action<IServiceProvider, IVkApi> configureClient)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services,
            Action<IVkApi> configureClient)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>. The client name will
        /// be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services,
            Action<IServiceProvider, IVkApi> configureClient)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var name = typeof(TClient).Name;
            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services, string name,
            Action<IVkApi> configureClient)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient>(this IServiceCollection services, string name,
            Action<IServiceProvider, IVkApi> configureClient)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services, string name,
            Action<IVkApi> configureClient)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="IVkApiFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient" /> type and a named <see cref="IVkApi"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. They type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedVkApiFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <typeparam name="TImplementation">
        /// The implementation type of the typed client. They type specified will be instantiated by the
        /// <see cref="ITypedVkApiFactory{TImplementation}"/>
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="IVkApi"/> to configure.</param>
        /// <param name="configureClient">A delegate that is used to configure an <see cref="IVkApi"/>.</param>
        /// <returns>An <see cref="IVkApiBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="IVkApi"/> instances that apply the provided configuration can be retrieved using 
        /// <see cref="IVkApiFactory.CreateVkApi(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="IVkApi" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type. 
        /// </para>
        /// <para>
        /// Use <see cref="Options.DefaultName"/> as the name to configure the default client.
        /// </para>
        /// </remarks>
        public static IVkApiBuilder AddVkApi<TClient, TImplementation>(this IServiceCollection services, string name,
            Action<IServiceProvider, IVkApi> configureClient)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            AddVkApi(services);

            var builder = new DefaultVkApiBuilder(services, name);
            builder.ConfigureVkApi(configureClient);
            builder.AddTypedVkApi<TClient, TImplementation>();
            return builder;
        }
    }
}
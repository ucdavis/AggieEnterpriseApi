﻿using System.Collections.Concurrent;
using AggieEnterpriseApi.Authentication;
using AggieEnterpriseApi.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace AggieEnterpriseApi;

public class GraphQlClient
{
    private static readonly ConcurrentDictionary<string, Lazy<IAggieEnterpriseClient>> _lazyClients = new();

    /// <summary>
    /// Returns an IAggieEnterpriseClient that uses consumer key and secret to generate new access tokens as needed
    /// </summary>
    /// <param name="queryEndpoint">URL for the actual GraphQL querying</param>
    /// <param name="tokenEndpoint">URL for OAuth Token grant endpoint</param>
    /// <param name="key">Consumer Key</param>
    /// <param name="secret">Consumer Secret</param>
    /// <param name="scope">Optional scope to avoid collisions. Each app should provide their own scope</param>
    /// <returns></returns>
    public static IAggieEnterpriseClient Get(string queryEndpoint, string tokenEndpoint, string key, string secret,
        string scope = "default")
    {
        var lazyClient = _lazyClients.GetOrAdd($"{queryEndpoint}_{tokenEndpoint}_{key}_{secret}_{scope}", clientKey =>
        {
            return new Lazy<IAggieEnterpriseClient>(() =>
            {
                var serviceCollection = new ServiceCollection();

                // add in general services
                serviceCollection.AddHttpClient();
                serviceCollection.AddMemoryCache();
                serviceCollection.AddTransient<AuthenticationDelegatingHandler>();
                serviceCollection.AddSingleton<ITokenService, TokenService>();

                // add in options so we can use them in our delegating handler
                serviceCollection.AddSingleton(new GraphQlClientOptions(queryEndpoint, tokenEndpoint, key, secret, scope));

                // add in serializers for custom int/float types (ex: PositiveInt)
                serviceCollection.AddSerializer<PositiveIntSerializer>();
                serviceCollection.AddSerializer<NonNegativeIntSerializer>();
                serviceCollection.AddSerializer<NonPositiveIntSerializer>();
                serviceCollection.AddSerializer<NonNegativeFloatSerializer>();

                serviceCollection
                    .AddAggieEnterpriseClient()
                    .ConfigureHttpClient((serviceProvider, client) =>
                    {
                        var options = serviceProvider.GetRequiredService<GraphQlClientOptions>();

                        client.BaseAddress = new Uri(options.QueryEndpoint);
                    }, builder =>
                    {
                        // add in our delegating handler to refresh the token if needed
                        builder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();
                    });

                IServiceProvider services = serviceCollection.BuildServiceProvider();

                return services.GetRequiredService<IAggieEnterpriseClient>();
            });
        });
        return lazyClient.Value;
    }

    public static IAggieEnterpriseClient Get(string url, string token)
    {
        var clientKey = $"{url}_{token}";

        var lazyClient = _lazyClients.GetOrAdd($"{url}_{token}", clientKey =>
        {
            return new Lazy<IAggieEnterpriseClient>(() =>
            {
                var graphQlUri = new Uri(url);

                var serviceCollection = new ServiceCollection();

                // add in serializers for custom int/float types (ex: PositiveInt)
                serviceCollection.AddSerializer<PositiveIntSerializer>();
                serviceCollection.AddSerializer<NonNegativeIntSerializer>();
                serviceCollection.AddSerializer<NonPositiveIntSerializer>();
                serviceCollection.AddSerializer<NonNegativeFloatSerializer>();

                serviceCollection
                    .AddAggieEnterpriseClient()
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = graphQlUri;
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    });

                IServiceProvider services = serviceCollection.BuildServiceProvider();

                return services.GetRequiredService<IAggieEnterpriseClient>();
            });
        });

        return lazyClient.Value;
    }
}
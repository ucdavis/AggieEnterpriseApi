using AggieEnterpriseApi.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace AggieEnterpriseApi;

public class GraphQlClient
{
    /// <summary>
    /// Returns an IAggieEnterpriseClient that uses consumer key and secret to generate new access tokens as needed
    /// </summary>
    /// <param name="url">URL for AE API</param>
    /// <param name="key">Consumer Key</param>
    /// <param name="secret">Consumer Secret</param>
    /// <param name="scope">Optional scope to avoid collisions. Each app should provide their own scope</param>
    /// <returns></returns>
    public static IAggieEnterpriseClient Get(string url, string key, string secret, string scope = "default")
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
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.BaseAddress = graphQlUri;
                var token = serviceProvider.GetRequiredService<TODO>();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            });

        IServiceProvider services = serviceCollection.BuildServiceProvider();

        return services.GetRequiredService<IAggieEnterpriseClient>();
    }
    
    public static IAggieEnterpriseClient Get(string url, string token)
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
    }
}

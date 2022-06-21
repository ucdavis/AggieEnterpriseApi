using AggieEnterpriseApi.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace AggieEnterpriseApi;

public class GraphQlClient
{
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
                client.DefaultRequestHeaders.Add("APIAuthToken", token);
            });

        IServiceProvider services = serviceCollection.BuildServiceProvider();

        return services.GetRequiredService<IAggieEnterpriseClient>();
    }
}

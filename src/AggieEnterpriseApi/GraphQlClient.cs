using Microsoft.Extensions.DependencyInjection;

namespace AggieEnterpriseApi;

public class GraphQlClient
{
    public static IAggieEnterpriseClient Get(string url, string token)
    {
        var graphQlUri = new Uri(url);

        var serviceCollection = new ServiceCollection();

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

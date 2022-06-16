namespace AggieEnterpriseApi.Extensions;

public static class DataExtensions
{
    public static T ReadData<T>(this StrawberryShake.IOperationResult<T>? result) where T : class
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (result.Errors.Count > 0)
        {
            throw new StrawberryShake.GraphQLClientException(result.Errors);
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("No data"); // TODO: TEST why data should ever be null
        }

        return result.Data;
    }
}
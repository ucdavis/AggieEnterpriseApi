# AggieEnterpriseApi

Api client library to interface with Aggie Enterprise GraphQL Api

Uses (Strawberry Shake)[https://chillicream.com/docs/strawberryshake/tooling] for GraphQL interface and object generation.

## GraphQL API Changes

If the API schema is changed and we need to update, run:

`dotnet graphql update`

See https://chillicream.com/docs/strawberryshake/tooling for more information

If you need to manually replace the schema, put it in schema.graphql

## Query changes

Queries (*.graphql) are compiled and injected on build (via roslyn).  So changes to queries don't require any special handling.

## Opt-in query retries

Existing callers keep retries disabled. To enable them for an application:

```csharp
var client = GraphQlClient.Get(queryEndpoint, tokenEndpoint, key, secret, scope,
    enableQueryRetries: true);

// Also supported for callers that supply their own bearer token:
var clientWithToken = GraphQlClient.Get(queryEndpoint, token, enableQueryRetries: true);
```

The policy retries HTTP 502, 503, and 504 responses at most twice, only for a
single GraphQL query operation. It parses the document instead of assuming POST
requests are safe. Mutations, subscriptions, batches, persisted-query-only requests,
unrecognized payloads, and documents with multiple operations receive no transient
retries. It does not retry HTTP 200 GraphQL errors, other HTTP statuses, or network
exceptions. The existing 401/403 token-refresh behavior is unchanged.

Backoff is 250–500 ms before the first retry and 500–1,000 ms before the second,
with random jitter. A longer `Retry-After` is honored up to five seconds; beyond
that the response is returned without retrying. Cancellation and the existing
HttpClient timeout apply across sends and backoff. Pass the caller's cancellation
token to `ExecuteAsync`; this feature does not introduce a new timeout setting.

The transient budget survives the existing token refresh, allowing at most four
GraphQL HTTP sends when both mechanisms are needed. Failed intermediate responses
are disposed. The last response is returned normally so existing error handling
still applies. Serilog records status codes, retry counts, and delays, without
request bodies, variables, credentials, or response bodies. Enabled and disabled
clients are cached separately.

Pilot this flag in Walter before enabling it in other apps. Avoid another retry
loop in callers. If retries are exhausted, the application should show temporary
unavailability and preserve authorization checks rather than treating missing
financial data as permission to proceed.

Run the offline retry tests without credentials or live API calls:

```sh
dotnet test test/ApiTests/ApiTests.csproj --filter FullyQualifiedName~ApiTests.QueryRetryTests
```

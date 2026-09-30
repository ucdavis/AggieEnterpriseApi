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

Retries are disabled by default. Enable them with:

```csharp
var client = GraphQlClient.Get(queryEndpoint, tokenEndpoint, key, secret, scope,
    enableQueryRetries: true);
```

The bearer-token overload also accepts `enableQueryRetries: true`. This retries
single read-only queries on HTTP 502/503/504 at most twice, with exponential
backoff and jitter. Mutations and ambiguous requests receive no transient retries.
Cancellation and the existing HTTP timeout still apply.

# AggieEnterpriseApi

Api client library to interface with Aggie Enterprise GraphQL Api

Uses (Strawberry Shake)[https://chillicream.com/docs/strawberryshake/tooling] for GraphQL interface and object generation.

## GraphQL API Changes

If the API schema is changed and we need to update, run:

`dotnet graphql update`

See https://chillicream.com/docs/strawberryshake/tooling for more information

## Query changes

Queries (*.graphql) are compiled and injected on build (via roslyn).  So changes to queries don't require any special handling.
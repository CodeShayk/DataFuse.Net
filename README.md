# <img src="https://github.com/CodeShayk/Schemio/blob/master/Images/data-integration-transparent.png" alt="data" style="width:50px;"/> DataFuse v2.1.0
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/Schemio/blob/master/LICENSE.md) 
[![Master-Build](https://github.com/CodeShayk/Schemio/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/CodeShayk/Schemio/actions/workflows/Build-Master.yml) 
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/Schemio?logo=github&sort=semver)](https://github.com/CodeShayk/Schemio/releases/latest)
[![Master-CodeQL](https://github.com/CodeShayk/Schemio/actions/workflows/Master-CodeQL.yml/badge.svg)](https://github.com/CodeShayk/Schemio/actions/workflows/Master-CodeQL.yml) 
[![.Net 9.0](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
--
DataFuse is a powerful .NET library designed to aggregate data from heterogeneous data stores using a schema-driven approach. It enables developers to hydrate complex object graphs by fetching data from multiple sources (SQL databases, Web APIs, NoSQL stores) using XPath and JSONPath schema mappings.
 #### Nuget Packages
| Package | Latest  | Details |
| --------| --------| --------|
| DataFuse.Integration|[![NuGet version](https://badge.fury.io/nu/DataFuse.Integration.svg)](https://badge.fury.io/nu/DataFuse.Integration) | Provides `core` functionality to configure nested queries and transformers. With ability to map schema paths (XPath/JSONPath) to entity's object graph. `No QueryEngine` provided and requires implementing IQueryEngine to execute IQuery instances. |
| DataFuse.Adapters.SQL|[![NuGet version](https://badge.fury.io/nu/DataFuse.Adapters.SQL.svg)](https://badge.fury.io/nu/DataFuse.Adapters.SQL)| Provides DataFuse with query engine using `Dapper` to execute SQL queries. |
| DataFuse.Adapters.EntityFramework|[![NuGet version](https://badge.fury.io/nu/DataFuse.Adapters.EntityFramework.svg)](https://badge.fury.io/nu/DataFuse.Adapters.EntityFramework)| Provides DataFuse with `Entity Framework` query engine to execute queries using DbContext. |
| DataFuse.Adapters.WebAPI|[![NuGet version](https://badge.fury.io/nu/DataFuse.Adapters.WebAPI.svg)](https://badge.fury.io/nu/DataFuse.Adapters.WebAPI)| Provides DataFuse with `Web Api` query engine to execute apis using HttpClient. |

## Concept
### What is DataFuse?
`DataFuse` is a data aggregation framework using queries that can target different data platforms.

Key benefits:
- allows fetching `aggregated` data from `heterogeneous` data storages. You could combine queries targetting different data platforms (example. `SQL`, `API`, `Cache`) to return an aggregated data `entity`.
- allows `conditional` fetching of `parts` of the aggregated data entity. You could retrieve parts of object graph in the aggregated entity by specifying schema paths (using `XPath` or `JSonPath`) to identify respective sections.

## Getting Started?
### i. Installation
Install the latest nuget package as appropriate for `Core`, `Web API`, `SQL` using `Dapper` or `EntityFramework` using commands below.

`DataFuse.Integration` - for installing DataFuse for `bespoke` implementation of query engine.
```
NuGet\Install-Package DataFuse.Integration
```
`DataFuse.Adapters.SQL` - for installing DataFuse for SQL with `Dapper` engine.
```
NuGet\Install-Package DataFuse.Adapters.SQL
```
`DataFuse.Adapters.EntityFramework` - for installing DataFuse for SQL with `EntityFramework` engine.
```
NuGet\Install-Package DataFuse.Adapters.EntityFramework
```
`DataFuse.Adapters.WebAPI` - for installing DataFuse for Web API with `HttpClient` engine.
```
NuGet\Install-Package DataFuse.Adapters.WebAPI
```
### ii. Developer Guide

Please see [Developer Guide](https://github.com/CodeShayk/Schemio/wiki) for complete details to use DataFuse in your project.

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/CodeShayk/Schemio/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).

## Version History
The main branch is now on .NET 9.0. The following previous versions are available:
| Version  | Release Notes | Developer Guide |
| -------- | --------|--------|
| [`v2.0.0`](https://github.com/CodeShayk/Schemio/tree/v2.0.0) |  [Notes](https://github.com/CodeShayk/Schemio/releases/tag/v2.0.0) | [Guide](https://github.com/CodeShayk/Schemio/blob/v2.0.0/index.md) |
| [`v1.0.0`](https://github.com/CodeShayk/Schemio/tree/v1.0.0) |  [Notes](https://github.com/CodeShayk/Schemio/releases/tag/v1.0.0) | [Guide](https://github.com/CodeShayk/Schemio/blob/v1.0.0/index.md) |

## Credits
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)





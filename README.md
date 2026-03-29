# <img src="https://github.com/CodeShayk/DataFuse.Net/blob/master/Images/data-integration-transparent.png" alt="data" style="width:50px;"/> DataFuse

### Like GraphQL, but for your heterogeneous backend systems

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/DataFuse.Net/blob/master/LICENSE.md)
[![Master-Build](https://github.com/CodeShayk/DataFuse.Net/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/CodeShayk/DataFuse.Net/actions/workflows/Build-Master.yml)
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/DataFuse.Net?logo=github&sort=semver)](https://github.com/CodeShayk/DataFuse.Net/releases/latest)
[![Master-CodeQL](https://github.com/CodeShayk/DataFuse.Net/actions/workflows/Master-CodeQL.yml/badge.svg)](https://github.com/CodeShayk/DataFuse.Net/actions/workflows/Master-CodeQL.yml)
[![.Net 9.0](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

**DataFuse** is a declarative framework that aggregates data from SQL databases, REST APIs, and any heterogeneous datsource into unified, strongly-typed objects — replacing hundreds of lines of manual orchestration code with clean, schema-driven configuration.

---

## The Problem

Building a product page? You need inventory from SQL Server, pricing from a REST API, and reviews from an external service. That's 50+ lines of manual fetch-assemble code — sequential, tightly coupled, and repeated across your codebase.

```csharp
// Without DataFuse: manual orchestration everywhere
var product = await db.QueryFirstAsync<ProductRecord>("SELECT * FROM Products WHERE Id = @Id", new { Id = productId });
var pricing = await httpClient.GetFromJsonAsync<PricingResponse>($"https://pricing-api/products/{productId}");
var reviews = await httpClient.GetFromJsonAsync<ReviewResponse[]>($"https://reviews-api/products/{productId}");

product.Price = pricing.Price;
product.Reviews = reviews.Select(r => new Review { Rating = r.Stars, Comment = r.Text }).ToArray();
```

## The Solution

```csharp
// With DataFuse: declare once, use everywhere
var product = dataProvider.GetData(new ProductRequest { ProductId = 42 });
```

DataFuse automatically executes the SQL query first, then runs pricing and reviews API calls **in parallel**, passes parent results to child queries, and assembles the final typed object.

---

## Quick Start

### 1. Install

```bash
dotnet add package DataFuse.Integration
dotnet add package DataFuse.Adapters.SQL       # SQL with Dapper
dotnet add package DataFuse.Adapters.WebAPI     # REST APIs
```

### 2. Define Your Entity

```csharp
public class Product : IEntity
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Review[] Reviews { get; set; }
}
```

### 3. Create Queries

```csharp
// SQL query — fetches product from database
public class ProductQuery : SQLQuery<ProductResult>
{
    protected override Func<IDbConnection, Task<ProductResult>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (ProductRequest)context.Request;
        return connection => connection.QueryFirstOrDefaultAsync<ProductResult>(
            "SELECT ProductId as Id, Name, Price FROM Products WHERE ProductId = @Id",
            new { Id = request.ProductId });
    }
}

// API query — fetches reviews from external service, receives parent result
public class ReviewsApiQuery : WebQuery<CollectionResult<ReviewResult>>
{
    public ReviewsApiQuery() : base("https://api.reviews.com/") { }

    protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        var product = (ProductResult)parentQueryResult;
        return () => new Uri($"products/{product.Id}/reviews", UriKind.Relative);
    }
}
```

### 4. Create Transformers

```csharp
public class ProductTransform : BaseTransformer<ProductResult, Product>
{
    public override void Transform(ProductResult queryResult, Product entity)
    {
        entity.ProductId = queryResult.Id;
        entity.Name = queryResult.Name;
        entity.Price = queryResult.Price;
    }
}

public class ReviewsTransform : BaseTransformer<CollectionResult<ReviewResult>, Product>
{
    public override void Transform(CollectionResult<ReviewResult> queryResult, Product entity)
    {
        entity.Reviews = queryResult?.Select(r => new Review
        {
            ReviewId = r.Id, Comment = r.Comment, Rating = r.Rating
        }).ToArray() ?? Array.Empty<Review>();
    }
}
```

### 5. Configure the Schema

```csharp
public class ProductConfiguration : EntityConfiguration<Product>
{
    public override IEnumerable<Mapping<Product, IQueryResult>> GetSchema()
    {
        return CreateSchema.For<Product>()
            .Map<ProductQuery, ProductTransform>(For.Paths("product"),
                product => product.Dependents
                    .Map<ReviewsApiQuery, ReviewsTransform>(For.Paths("product/reviews")))
            .End();
    }
}
```

### 6. Register & Use

```csharp
// DI registration
services.UseDataFuse()
    .WithEngine(c => new QueryEngine(sqlConfiguration))
    .WithEngine<DataFuse.Adapters.WebAPI.QueryEngine>()
    .WithPathMatcher(c => new XPathMatcher())
    .WithEntityConfiguration<Product>(c => new ProductConfiguration());

services.AddHttpClient();
```

```csharp
// Usage — one line to get fully hydrated data
public class ProductService
{
    private readonly IDataProvider<Product> _dataProvider;

    public ProductService(IDataProvider<Product> dataProvider) => _dataProvider = dataProvider;

    public Product GetProduct(int productId)
        => _dataProvider.GetData(new ProductRequest { ProductId = productId });

    // Selective loading — only fetch product + reviews, skip other paths
    public Product GetProductWithReviews(int productId)
        => _dataProvider.GetData(new ProductRequest
        {
            ProductId = productId,
            SchemaPaths = new[] { "product", "product/reviews" }
        });
}
```

---

## Key Features

| Feature | Description |
|---|---|
| **Declarative Schema** | Define data relationships once with fluent configuration |
| **Automatic Parallelism** | Sibling queries run in parallel — no `Task.WhenAll` boilerplate |
| **Selective Loading** | Fetch only the data paths the consumer needs via `SchemaPaths` |
| **Parent-Child Dependencies** | Parent results flow to child queries automatically |
| **Cross-Source Mixing** | Combine SQL + REST API + EF Core queries in one entity |
| **Type Safety** | Strongly-typed queries, results, and transformers |
| **Built-in Caching** | `[CacheResult]` attribute for expensive query results |
| **Custom Adapters** | Add any data source by implementing `IQueryEngine` |

---

## Packages

| Package | Purpose | Install |
|---|---|---|
| **DataFuse.Integration** | Core orchestration, DI, helpers | `dotnet add package DataFuse.Integration` |
| **DataFuse.Adapters.SQL** | SQL via Dapper | `dotnet add package DataFuse.Adapters.SQL` |
| **DataFuse.Adapters.EntityFramework** | EF Core | `dotnet add package DataFuse.Adapters.EntityFramework` |
| **DataFuse.Adapters.WebAPI** | REST APIs via HttpClient | `dotnet add package DataFuse.Adapters.WebAPI` |
| **DataFuse.Adapters.Abstraction** | Interfaces & base classes | Included as dependency |

### Compatibility

| Package | .NET | .NET Standard | .NET Framework |
|---|---|---|---|
| DataFuse.Integration | 9.0+ | 2.0, 2.1 | 4.6.2+ |
| DataFuse.Adapters.SQL | 9.0+ | 2.1 | 4.6.2+ |
| DataFuse.Adapters.EntityFramework | 9.0+ | - | - |
| DataFuse.Adapters.WebAPI | 9.0+ | 2.0, 2.1 | 4.6.2+ |

---

## Documentation

See the [Complete Developer Guide](https://codeshayk.github.io/DataFuse.Net/) for detailed documentation including:
- Real-world use cases (e-commerce, customer 360, reporting)
- Core concepts deep dive
- Query implementation guides (SQL, EF Core, Web API)
- Transformer patterns
- Advanced features (caching, selective loading, custom engines)
- Architecture overview

---

## Support

If you are having problems, please let us know by [raising a new issue](https://github.com/CodeShayk/DataFuse.Net/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).

## Version History

The main branch is now on .NET 9.0. Previous versions:

| Version | Release Notes | Developer Guide |
|---|---|---|
| [`v2.0.0`](https://github.com/CodeShayk/DataFuse.Net/tree/v2.0.0) | [Notes](https://github.com/CodeShayk/DataFuse.Net/releases/tag/v2.0.0) | [Guide](https://github.com/CodeShayk/DataFuse.Net/blob/v2.0.0/index.md) |
| [`v1.0.0`](https://github.com/CodeShayk/DataFuse.Net/tree/v1.0.0) | [Notes](https://github.com/CodeShayk/DataFuse.Net/releases/tag/v1.0.0) | [Guide](https://github.com/CodeShayk/DataFuse.Net/blob/v1.0.0/index.md) |

# DataFuse Framework

### Like GraphQL, but for your heterogeneous backend systems

**DataFuse** is a declarative .NET framework that aggregates data from SQL databases, REST APIs, and Entity Framework into unified, strongly-typed objects — replacing hundreds of lines of manual orchestration code with a clean, schema-driven configuration.

[![.Net 9.0](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/DataFuse/blob/master/LICENSE.md)
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/DataFuse?logo=github&sort=semver)](https://github.com/CodeShayk/DataFuse/releases/latest)

---

## Table of Contents

1. [Real-World Use Cases](#real-world-use-cases)
2. [Why DataFuse?](#why-datafuse)
3. [Quick Start (5 Minutes)](#quick-start)
4. [Core Concepts](#core-concepts)
5. [Packages](#packages)
6. [Query Implementation Guide](#query-implementation-guide)
7. [Transformer Guide](#transformer-guide)
8. [Advanced Features](#advanced-features)
9. [Architecture](#architecture)
10. [Best Practices](#best-practices)
11. [Getting Support](#getting-support)

---

## Real-World Use Cases

### 1. E-Commerce Product Page

Your product page needs data from **three different systems**: inventory from a SQL Server database, live pricing from a pricing microservice REST API, and customer reviews from an external review platform.

**Without DataFuse** — you write this every time:

```csharp
// 50+ lines of manual orchestration, repeated across your codebase
public async Task<ProductPage> GetProductPage(int productId)
{
    var product = await _db.QueryFirstAsync<ProductRecord>(
        "SELECT * FROM Products WHERE Id = @Id", new { Id = productId });

    var pricing = await _httpClient.GetFromJsonAsync<PricingResponse>(
        $"https://pricing-api/products/{productId}");

    var reviews = await _httpClient.GetFromJsonAsync<ReviewResponse[]>(
        $"https://reviews-api/products/{productId}");

    // Manual assembly — error-prone, no caching, no parallel execution
    return new ProductPage
    {
        ProductId = product.Id,
        Name = product.Name,
        Description = product.Description,
        CurrentPrice = pricing.Price,
        DiscountPercent = pricing.Discount,
        Reviews = reviews.Select(r => new Review
        {
            Rating = r.Stars,
            Comment = r.Text,
            Author = r.AuthorName
        }).ToArray(),
        AverageRating = reviews.Average(r => r.Stars)
    };
}
```

**With DataFuse** — declare your schema once, use everywhere:

```csharp
public class ProductPageConfiguration : EntityConfiguration<ProductPage>
{
    public override IEnumerable<Mapping<ProductPage, IQueryResult>> GetSchema()
    {
        return CreateSchema.For<ProductPage>()
            .Map<ProductQuery, ProductTransform>(For.Paths("product"),
                product => product.Dependents
                    .Map<PricingApiQuery, PricingTransform>(For.Paths("product/pricing"))
                    .Map<ReviewsApiQuery, ReviewsTransform>(For.Paths("product/reviews")))
            .End();
    }
}

// Usage — one line to get fully hydrated data
var productPage = dataProvider.GetData(new ProductRequest { ProductId = 42 });
```

DataFuse automatically:
- Executes the SQL product query first
- Runs pricing and reviews API calls **in parallel** (they're siblings)
- Passes the product result to dependent queries so they know which product to fetch
- Assembles the final `ProductPage` object via type-safe transformers

### 2. Customer 360 Dashboard

Build a unified customer view pulling from a CRM database, a billing REST API, and a support ticket system:

```csharp
public class Customer360Configuration : EntityConfiguration<Customer360>
{
    public override IEnumerable<Mapping<Customer360, IQueryResult>> GetSchema()
    {
        return CreateSchema.For<Customer360>()
            .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
                customer => customer.Dependents
                    .Map<BillingApiQuery, BillingTransform>(For.Paths("customer/billing"))
                    .Map<TicketsApiQuery, TicketsTransform>(For.Paths("customer/tickets"))
                    .Map<OrdersQuery, OrdersTransform>(For.Paths("customer/orders"),
                        orders => orders.Dependents
                            .Map<OrderItemsQuery, OrderItemsTransform>(
                                For.Paths("customer/orders/order/items"))))
            .End();
    }
}
```

Need just the basic profile without orders? Use **selective loading**:

```csharp
var basicProfile = dataProvider.GetData(new CustomerRequest
{
    CustomerId = 123,
    SchemaPaths = new[] { "customer", "customer/billing" }
});
```

### 3. Multi-Service Reporting

Aggregate metrics from multiple microservices into a single report object, mixing SQL databases with REST APIs:

```csharp
public class SalesReportConfiguration : EntityConfiguration<SalesReport>
{
    public override IEnumerable<Mapping<SalesReport, IQueryResult>> GetSchema()
    {
        return CreateSchema.For<SalesReport>()
            .Map<SalesSummaryQuery, SalesSummaryTransform>(For.Paths("report"),
                report => report.Dependents
                    .Map<InventoryApiQuery, InventoryTransform>(For.Paths("report/inventory"))
                    .Map<ShippingApiQuery, ShippingTransform>(For.Paths("report/shipping"))
                    .Map<RevenueQuery, RevenueTransform>(For.Paths("report/revenue")))
            .End();
    }
}
```

---

## Why DataFuse?

### The Problem

In modern architectures, a single page or API response often needs data from **multiple backend systems** — SQL databases, REST APIs, third-party services, caches. The standard approach is to write manual orchestration code for each scenario. This leads to:

- **Boilerplate explosion**: Every new data combination means another 50+ lines of fetch-assemble-transform code
- **No parallelism by default**: Developers write sequential calls unless they manually add `Task.WhenAll`
- **Tight coupling**: Changing a data source means rewriting orchestration code
- **No selective loading**: You fetch everything even when the consumer only needs a subset
- **Inconsistent patterns**: Every developer solves the same problem differently

### How DataFuse Solves This

DataFuse provides a **declarative, schema-driven approach** where you:

1. **Define** your entity (the shape of the data you want)
2. **Configure** a schema mapping queries to entity paths
3. **Let DataFuse handle** execution order, parallelism, dependency passing, and assembly

### DataFuse vs Alternatives

| Capability | Manual Code | GraphQL | MediatR | **DataFuse** |
|---|---|---|---|---|
| Multi-source aggregation | Manual wiring | Resolver-based | Manual wiring | **Declarative schema** |
| Parallel execution | Manual `Task.WhenAll` | Per-resolver | Manual | **Automatic** |
| Selective loading | Manual if/else | Built-in | Manual | **Schema path filtering** |
| Dependency management | Manual ordering | Implicit | Manual | **Parent-child hierarchy** |
| Type safety | Varies | Schema-based | Yes | **Strongly typed** |
| New data source support | Rewrite orchestration | New resolver | New handler | **Add adapter** |
| Learning curve | None | High (new language) | Low | **Low (C# only)** |
| Backend-only (no client changes) | Yes | No (client queries) | Yes | **Yes** |

### When to Use DataFuse

**Use DataFuse when:**
- Your data lives across **different systems** (SQL + APIs + external services)
- You need to **compose complex object graphs** from multiple sources
- You want **automatic parallelism** without managing it yourself
- You need **selective loading** — different consumers need different subsets of data
- You're building **API gateways**, **BFF patterns**, or **composite APIs**

**Use plain Dapper/EF Core when:**
- All your data is in a single database
- Simple queries with SQL joins suffice

**Use GraphQL when:**
- You need client-controlled query shapes
- You're building a public API where consumers define what they need

---

## Quick Start

Get up and running in 5 minutes.

### 1. Install Packages

```bash
dotnet add package DataFuse.Integration
dotnet add package DataFuse.Adapters.SQL          # For SQL with Dapper
dotnet add package DataFuse.Adapters.WebAPI        # For REST APIs
dotnet add package DataFuse.Adapters.EntityFramework  # For EF Core
```

### 2. Define Your Entity

```csharp
public class Product : IEntity
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }
    public Review[] Reviews { get; set; }
}
```

### 3. Create Queries

**SQL query** (fetches product from database):
```csharp
public class ProductQuery : SQLQuery<ProductResult>
{
    protected override Func<IDbConnection, Task<ProductResult>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (ProductRequest)context.Request;
        return connection => connection.QueryFirstOrDefaultAsync<ProductResult>(
            "SELECT ProductId as Id, Name, Price, CategoryId FROM Products WHERE ProductId = @Id",
            new { Id = request.ProductId });
    }
}
```

**API query** (fetches reviews from external service):
```csharp
public class ReviewsApiQuery : WebQuery<CollectionResult<ReviewResult>>
{
    public ReviewsApiQuery() : base("https://api.reviews.com/") { }

    protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        var product = (ProductResult)parentQueryResult; // Parent result is available
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
                    .Map<CategoryQuery, CategoryTransform>(For.Paths("product/category"))
                    .Map<ReviewsApiQuery, ReviewsTransform>(For.Paths("product/reviews")))
            .End();
    }
}
```

### 6. Register with DI

```csharp
services.UseDataFuse()
    .WithEngine(c => new QueryEngine(sqlConfiguration))        // SQL adapter
    .WithEngine<DataFuse.Adapters.WebAPI.QueryEngine>()        // Web API adapter
    .WithPathMatcher(c => new XPathMatcher())
    .WithEntityConfiguration<Product>(c => new ProductConfiguration());

services.AddHttpClient();
```

### 7. Use It

```csharp
public class ProductService
{
    private readonly IDataProvider<Product> _dataProvider;

    public ProductService(IDataProvider<Product> dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public Product GetProduct(int productId)
    {
        return _dataProvider.GetData(new ProductRequest { ProductId = productId });
    }

    // Selective loading — only fetch product + reviews, skip category
    public Product GetProductWithReviews(int productId)
    {
        return _dataProvider.GetData(new ProductRequest
        {
            ProductId = productId,
            SchemaPaths = new[] { "product", "product/reviews" }
        });
    }
}
```

---

## Core Concepts

### Entities

Entities represent the final aggregated data structure. They implement `IEntity` and define the complete object graph that DataFuse will hydrate from multiple sources:

```csharp
public class Customer : IEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; }

    // Each of these can come from a different data source
    public Communication Communication { get; set; }  // From REST API
    public Address Address { get; set; }                // From SQL database
    public Order[] Orders { get; set; }                 // From EF Core
}
```

### Queries

Queries fetch data from a specific source. DataFuse provides base classes for common sources:

| Base Class | Source | Adapter Package |
|---|---|---|
| `SQLQuery<TResult>` | SQL databases via Dapper | `DataFuse.Adapters.SQL` |
| `SQLQuery<TResult>` (EF) | Entity Framework Core | `DataFuse.Adapters.EntityFramework` |
| `WebQuery<TResult>` | REST APIs via HttpClient | `DataFuse.Adapters.WebAPI` |

#### Parent-Child Query Dependencies

Child queries receive their parent's result, enabling dependent data fetching:

```csharp
// Root query — fetches customer from database
public class CustomerQuery : SQLQuery<CustomerResult>
{
    protected override Func<IDbConnection, Task<CustomerResult>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (CustomerRequest)context.Request;
        return connection => connection.QueryFirstOrDefaultAsync<CustomerResult>(
            "SELECT CustomerId as Id, Name, Code FROM Customers WHERE CustomerId = @Id",
            new { Id = request.CustomerId });
    }
}

// Child query — uses parent's CustomerId to fetch orders
public class OrdersQuery : SQLQuery<CollectionResult<OrderResult>>
{
    protected override Func<IDbConnection, Task<CollectionResult<OrderResult>>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var customer = (CustomerResult)parentQueryResult;
        return async connection =>
        {
            var orders = await connection.QueryAsync<OrderResult>(
                "SELECT OrderId, OrderNumber, OrderDate FROM Orders WHERE CustomerId = @Id",
                new { Id = customer.Id });
            return new CollectionResult<OrderResult>(orders);
        };
    }
}
```

#### Execution Order

```
1. Root Query (CustomerQuery) executes first
   |
   v  CustomerResult passed to children

2. Child Queries execute IN PARALLEL:
   - OrdersQuery (uses CustomerId)
   - CommunicationQuery (uses CustomerId)
   - AddressQuery (uses CustomerId)
   |
   v  OrderResult passed to grandchildren

3. Grandchild Queries execute:
   - OrderItemsQuery (uses OrderId from OrderResult)
```

Queries at the same level run in parallel automatically. DataFuse manages the dependency tree.

### Transformers

Transformers map query results onto the entity. Each query has a paired transformer:

```csharp
public class CustomerTransform : BaseTransformer<CustomerResult, Customer>
{
    public override void Transform(CustomerResult queryResult, Customer entity)
    {
        entity.CustomerId = queryResult.Id;
        entity.Name = queryResult.Name;
    }
}

public class OrdersTransform : BaseTransformer<CollectionResult<OrderResult>, Customer>
{
    public override void Transform(CollectionResult<OrderResult> queryResult, Customer entity)
    {
        entity.Orders = queryResult?.Select(o => new Order
        {
            OrderId = o.OrderId,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate
        }).ToArray() ?? Array.Empty<Order>();
    }
}
```

### Schema Configuration

The schema defines the full query-transform-path hierarchy:

```csharp
public class CustomerConfiguration : EntityConfiguration<Customer>
{
    public override IEnumerable<Mapping<Customer, IQueryResult>> GetSchema()
    {
        return CreateSchema.For<Customer>()
            .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
                customer => customer.Dependents
                    .Map<CommunicationQuery, CommunicationTransform>(
                        For.Paths("customer/communication"))
                    .Map<OrdersQuery, OrdersTransform>(For.Paths("customer/orders"),
                        orders => orders.Dependents
                            .Map<OrderItemsQuery, OrderItemsTransform>(
                                For.Paths("customer/orders/order/items"))))
            .End();
    }
}
```

Schema paths correspond to the entity's object graph:
- `customer` — root properties
- `customer/communication` — Customer.Communication
- `customer/orders` — Customer.Orders collection
- `customer/orders/order/items` — Order.Items for each order

---

## Packages

### DataFuse.Integration

Core package with orchestration, DI extensions, helpers, and path matchers.

```bash
dotnet add package DataFuse.Integration
```

**Key components:** `DataProvider<T>`, `QueryBuilder<T>`, `EntityBuilder<T>`, `DataFuseOptionsBuilder`, XPath/JSONPath matchers, DI registration via `UseDataFuse()`.

### DataFuse.Adapters.Abstraction

Interfaces and base classes: `IEntity`, `IQuery`, `IQueryResult`, `ITransformer`, `IQueryEngine`, `BaseQuery`, `BaseTransformer`, `EntityConfiguration<T>`, `CreateSchema`, `CollectionResult<T>`, `CacheResultAttribute`.

### DataFuse.Adapters.SQL

SQL database support using Dapper.

```bash
dotnet add package DataFuse.Adapters.SQL
```

Supports SQL Server, SQLite, MySQL, PostgreSQL, Oracle.

### DataFuse.Adapters.EntityFramework

Entity Framework Core integration.

```bash
dotnet add package DataFuse.Adapters.EntityFramework
```

Full LINQ support, DbContext factory integration.

### DataFuse.Adapters.WebAPI

REST API support using HttpClient.

```bash
dotnet add package DataFuse.Adapters.WebAPI
```

Request/response header management, JSON deserialization.

### Compatibility

| Package | .NET Framework | .NET Standard | .NET |
|---|---|---|---|
| DataFuse.Integration | 4.6.2+ | 2.0, 2.1 | 9.0+ |
| DataFuse.Adapters.SQL | 4.6.2+ | 2.1 | 9.0+ |
| DataFuse.Adapters.EntityFramework | - | - | 9.0+ |
| DataFuse.Adapters.WebAPI | 4.6.2+ | 2.0, 2.1 | 9.0+ |

---

## Query Implementation Guide

### SQL Queries (Dapper)

```csharp
// Single result
public class CustomerQuery : SQLQuery<CustomerResult>
{
    protected override Func<IDbConnection, Task<CustomerResult>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (CustomerRequest)context.Request;
        return connection => connection.QueryFirstOrDefaultAsync<CustomerResult>(
            @"SELECT CustomerId as Id, CustomerName as Name, CustomerCode as Code
              FROM Customers WHERE CustomerId = @CustomerId",
            new { CustomerId = request.CustomerId });
    }
}

// Collection result
public class OrdersQuery : SQLQuery<CollectionResult<OrderResult>>
{
    protected override Func<IDbConnection, Task<CollectionResult<OrderResult>>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var customer = (CustomerResult)parentQueryResult;
        return async connection =>
        {
            var orders = await connection.QueryAsync<OrderResult>(
                "SELECT OrderId, OrderNumber, OrderDate FROM Orders WHERE CustomerId = @Id",
                new { Id = customer.Id });
            return new CollectionResult<OrderResult>(orders);
        };
    }
}
```

### Entity Framework Queries

```csharp
public class CustomerQuery : SQLQuery<CustomerResult>
{
    protected override Func<DbContext, Task<CustomerResult>> GetQuery(
        IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (CustomerRequest)context.Request;
        return async dbContext =>
        {
            return await dbContext.Set<CustomerEntity>()
                .Where(c => c.CustomerId == request.CustomerId)
                .Select(c => new CustomerResult
                {
                    Id = c.CustomerId,
                    Name = c.Name,
                    Code = c.Code
                })
                .FirstOrDefaultAsync();
        };
    }
}
```

### Web API Queries

```csharp
// Basic API query
public class PricingApiQuery : WebQuery<PricingResult>
{
    public PricingApiQuery() : base("https://api.pricing.com/") { }

    protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        var product = (ProductResult)parentQueryResult;
        return () => new Uri($"products/{product.Id}/pricing", UriKind.Relative);
    }
}

// API query with custom headers
public class AuthenticatedApiQuery : WebQuery<UserDataResult>
{
    public AuthenticatedApiQuery() : base("https://api.secure.com/") { }

    protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        var request = (SecureRequest)context.Request;
        return () => new Uri($"secure/data/{request.Id}", UriKind.Relative);
    }

    protected override IDictionary<string, string> GetRequestHeaders()
    {
        return new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + GetAccessToken() },
            { "Accept", "application/json" }
        };
    }

    protected override IEnumerable<string> GetResponseHeaders()
    {
        return new[] { "X-Rate-Limit-Remaining", "X-Request-Id" };
    }
}
```

### Query Result Types

```csharp
// Simple result
public class CustomerResult : IQueryResult
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}

// Collection result
public class CollectionResult<T> : List<T>, IQueryResult
{
    public CollectionResult(IEnumerable<T> items) : base(items) { }
    public CollectionResult() { }
}

// Cached result — cached automatically across queries
[CacheResult]
public class CategoryResult : IQueryResult
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Web result with response headers
public class UserApiResult : WebHeaderResult
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

---

## Transformer Guide

### Basic Transformers

```csharp
public class CustomerTransform : BaseTransformer<CustomerResult, Customer>
{
    public override void Transform(CustomerResult queryResult, Customer entity)
    {
        if (queryResult == null) return;

        entity.CustomerId = queryResult.Id;
        entity.CustomerName = queryResult.Name ?? string.Empty;
        entity.CustomerCode = queryResult.Code;
    }
}
```

### Collection Transformers

```csharp
public class OrdersTransform : BaseTransformer<CollectionResult<OrderResult>, Customer>
{
    public override void Transform(CollectionResult<OrderResult> queryResult, Customer entity)
    {
        entity.Orders = queryResult?.Select(o => new Order
        {
            OrderId = o.OrderId,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount
        }).ToArray() ?? Array.Empty<Order>();
    }
}
```

### Context-Aware Transformers

Transformers can access the request context for conditional logic:

```csharp
public class ProductTransform : BaseTransformer<ProductResult, Product>
{
    public override void Transform(ProductResult queryResult, Product entity)
    {
        entity.ProductId = queryResult.Id;
        entity.Name = queryResult.Name;
        entity.Price = queryResult.Price;

        var request = Context.Request as ProductRequest;
        if (request?.IncludeMetadata == true)
        {
            entity.CreatedDate = queryResult.CreatedDate;
            entity.LastModified = queryResult.LastModified;
        }
    }
}
```

### Grandchild Transformers

For deeply nested data, transformers map grandchild results back to the correct parent:

```csharp
public class OrderItemsTransform : BaseTransformer<CollectionResult<OrderItemResult>, Customer>
{
    public override void Transform(CollectionResult<OrderItemResult> queryResult, Customer entity)
    {
        if (queryResult == null || !queryResult.Any()) return;

        var itemsByOrder = queryResult.GroupBy(item => item.OrderId);
        foreach (var group in itemsByOrder)
        {
            var order = entity.Orders.FirstOrDefault(o => o.OrderId == group.Key);
            if (order != null)
            {
                order.Items = group.Select(item => new OrderItem
                {
                    ItemId = item.ItemId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToArray();
            }
        }
    }
}
```

---

## Advanced Features

### Caching

Mark query results with `[CacheResult]` to cache them for use in other queries and transformers:

```csharp
[CacheResult]
public class CategoryResult : IQueryResult
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Access cached results in a transformer
public class ProductTransform : BaseTransformer<ProductResult, Product>
{
    public override void Transform(ProductResult queryResult, Product entity)
    {
        entity.ProductId = queryResult.Id;

        if (Context.Cache.TryGetValue("CategoryResult", out var cached))
        {
            entity.CategoryName = ((CategoryResult)cached).Name;
        }
    }
}
```

Caching is useful when a grandchild query needs data from a grandparent query that isn't its direct parent.

### Selective Loading

Control which parts of the object graph to load via `SchemaPaths`:

```csharp
// Load everything
var fullCustomer = dataProvider.GetData(new CustomerRequest
{
    CustomerId = 123
    // SchemaPaths = null loads all configured paths
});

// Load only customer + orders (skip communication, address, etc.)
var withOrders = dataProvider.GetData(new CustomerRequest
{
    CustomerId = 123,
    SchemaPaths = new[] { "customer", "customer/orders" }
});

// Load just the basic profile
var basicProfile = dataProvider.GetData(new CustomerRequest
{
    CustomerId = 123,
    SchemaPaths = new[] { "customer" }
});
```

This avoids unnecessary queries — if a consumer only needs the customer name, DataFuse won't execute order or communication queries.

### Parallel Execution

Sibling queries at the same level execute in parallel automatically:

```csharp
return CreateSchema.For<Customer>()
    .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
        customer => customer.Dependents
            .Map<ContactQuery, ContactTransform>(For.Paths("customer/contact"))           // Parallel
            .Map<PreferencesQuery, PreferencesTransform>(For.Paths("customer/preferences")) // Parallel
            .Map<AddressQuery, AddressTransform>(For.Paths("customer/address")))           // Parallel
    .End();
```

No `Task.WhenAll` boilerplate needed. DataFuse manages execution order based on the dependency tree.

### Conditional Query Execution

Child queries can skip execution based on parent data:

```csharp
public class PremiumServicesQuery : WebQuery<PremiumServicesResult>
{
    protected override Func<Uri> GetQuery(IDataContext context, IQueryResult parentQueryResult)
    {
        var customer = (CustomerResult)parentQueryResult;

        if (customer.CustomerType != "Premium")
            return null; // Returning null skips this query

        return () => new Uri($"premium-services/{customer.Id}", UriKind.Relative);
    }
}
```

### Cross-Source Dependencies

Mix data sources within the same entity. A root query can be SQL while its children are REST APIs:

```csharp
return CreateSchema.For<Customer>()
    .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),        // SQL
        customer => customer.Dependents
            .Map<BillingApiQuery, BillingTransform>(For.Paths("customer/billing"))   // REST API
            .Map<AnalyticsQuery, AnalyticsTransform>(For.Paths("customer/analytics"))) // Different DB
    .End();
```

### Custom Query Engines

Add support for any data source by implementing `IQueryEngine`:

```csharp
public class MongoQueryEngine : IQueryEngine
{
    private readonly IMongoDatabase _database;

    public MongoQueryEngine(IMongoDatabase database)
    {
        _database = database;
    }

    public bool CanExecute(IQuery query) => query is IMongoQuery;

    public async Task<IQueryResult> Execute(IQuery query)
    {
        var mongoQuery = (IMongoQuery)query;
        return await mongoQuery.Execute(_database);
    }
}

// Register it
services.UseDataFuse()
    .WithEngine(c => new MongoQueryEngine(mongoDatabase))
    .WithEngine(c => new QueryEngine(sqlConfiguration))
    // ...
```

### Path Matching

DataFuse supports XPath and JSONPath patterns for schema paths:

**XPath patterns:**
- `customer` — exact match
- `customer/orders` — nested path
- `customer/orders/order/items` — deep nesting

**JSONPath patterns:**
- `$.customer` — root level
- `$.customer.orders` — nested property
- `$.customer.orders[*].items` — array elements

Register your preferred matcher:
```csharp
services.UseDataFuse()
    .WithPathMatcher(c => new XPathMatcher())   // or JPathMatcher
```

---

## Architecture

### Execution Flow

```
Client Request
    |
    v
DataProvider<T>
    |
    v
QueryBuilder — filters by SchemaPaths, resolves dependencies, builds execution plan
    |
    v
QueryExecutor — executes level-by-level, parallelizes siblings, propagates results
    |
    v
EntityBuilder — applies transformers in dependency order, assembles final entity
    |
    v
Fully Hydrated Entity
```

### Component Responsibilities

| Component | Responsibility |
|---|---|
| `DataProvider<T>` | Main orchestrator — coordinates the full pipeline |
| `QueryBuilder<T>` | Filters queries by requested schema paths, resolves parent-child dependencies |
| `QueryExecutor` | Executes queries level-by-level with parallel processing of siblings |
| `EntityBuilder<T>` | Matches results to transformers and builds the final entity |
| `IQueryEngine` | Adapter interface — each data source implements this |
| `EntityConfiguration<T>` | Declares the schema: query-transform-path mappings |

### Configuration Patterns

**Linear** — flat entity:
```csharp
return CreateSchema.For<Customer>()
    .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"))
    .Map<AddressQuery, AddressTransform>(For.Paths("customer/address"))
    .End();
```

**Branching** — multiple children:
```csharp
return CreateSchema.For<Customer>()
    .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
        c => c.Dependents
            .Map<ContactQuery, ContactTransform>(For.Paths("customer/contact"))
            .Map<PreferencesQuery, PrefsTransform>(For.Paths("customer/preferences")))
    .End();
```

**Deep nesting** — up to 5 levels:
```csharp
return CreateSchema.For<Customer>()
    .Map<CustomerQuery, CustomerTransform>(For.Paths("customer"),
        c => c.Dependents
            .Map<OrdersQuery, OrdersTransform>(For.Paths("customer/orders"),
                o => o.Dependents
                    .Map<ItemsQuery, ItemsTransform>(For.Paths("customer/orders/order/items"))))
    .End();
```

---

## Best Practices

### Query Design
- Keep queries focused — one query per data concern
- Use `CollectionResult<T>` for queries returning multiple records
- Return `null` from `GetQuery` to conditionally skip execution
- Use `[CacheResult]` for expensive, reusable query results

### Transformer Design
- Always handle null query results gracefully
- Initialize collections to prevent null reference exceptions
- Keep transformers focused on mapping — avoid business logic
- Use `Context.Request` for conditional transformation

### Schema Design
- Use meaningful, hierarchical path names that mirror your entity structure
- Keep nesting depth reasonable (3-4 levels is typical)
- Use selective loading for performance-sensitive endpoints

### DI Registration
```csharp
services.UseDataFuse()
    .WithEngine(c => new QueryEngine(sqlConfig))
    .WithEngine<DataFuse.Adapters.WebAPI.QueryEngine>()
    .WithPathMatcher(c => new XPathMatcher())
    .WithEntityConfiguration<Customer>(c => new CustomerConfiguration())
    .WithEntityConfiguration<Product>(c => new ProductConfiguration());

services.AddLogging();
services.AddHttpClient();
```

---

## Getting Support

- **GitHub**: [github.com/CodeShayk/DataFuse](https://github.com/CodeShayk/DataFuse)
- **Issues**: [Report bugs and feature requests](https://github.com/CodeShayk/DataFuse/issues)
- **Wiki**: [Developer guide and documentation](https://github.com/CodeShayk/DataFuse/wiki)
- **Samples**: Check example projects in the repository for real-world usage patterns

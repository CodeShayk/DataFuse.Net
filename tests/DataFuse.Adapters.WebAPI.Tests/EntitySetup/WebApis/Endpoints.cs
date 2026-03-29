namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.WebApis
{
    public static class Endpoints
    {
        public const string Customer = "v2/clients/{0}";
        public const string Communication = "v2/clients/{0}/communication";
        public const string Orders = "v2/clients/{0}/orders";
        public const string OrderItems = "v2/clients/{0}/orders/items?$filter=orderId in {1}";

        public const string BaseAddress = "http://localhost:5000/";

        public static class Ids
        {
            public const int CustomerId = 1000;
            public const int OrderId = 1234;
        }

        public static class Response
        {
            public const string Customer = @"{""id"": 1000, ""code"": ""ABC-2244"",""name"": ""John McKinsy""}";
            public const string Communication = @"{ 'contactId': 4567, 'phone': '07675998878', 'email': 'John.McKinsy@gmail.com', 'postalAddress': { 'addressId': 3456, 'houseNo': '22', 'city': 'London', 'region': 'London', 'postalCode': 'W12 6GH', 'country': 'United Kingdom' } }";
            public const string Orders = @"{ 'orderId': 1234, 'orderNo': 'GHK-897GB', 'date': '2024-01-01T00:00:00' }";
            public const string OrderItems = @"[{ 'itemId': 2244, 'name': 'Pen', 'cost': 12.0 }, { 'itemId': 6677, 'name': 'Book', 'cost': 15.0 }]";
        }
    }
}
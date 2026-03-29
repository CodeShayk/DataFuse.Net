namespace DataFuse.Adapters.WebAPI.Tests.EntitySetup.QueryResults
{
    public class CustomerResult : WebHeaderResult
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
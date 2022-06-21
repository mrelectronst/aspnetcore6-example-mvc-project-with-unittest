namespace AspNetCoreMVCxUnitTest.Web.Database.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }    

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}

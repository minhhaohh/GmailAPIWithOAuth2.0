namespace GmailAPIWithOAuth2.Models
{
    public class Person
    {
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }

    public class Order
    {
        public string OrderNumber { get; set; }

        public Person Customer { get; set; }

        public List<Product> Products { get; set; }
    }
}

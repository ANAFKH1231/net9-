using Microsoft.Data.SqlClient;
using net9购物网站.Data;
using net9购物网站.MODEL;

namespace net9购物网站.Pages
{
    public static class ProductService
    {
        public static List<Product> GetProducts(AppDbContext context)
        {
            if (context == null)
            {
                throw new InvalidOperationException("The AppDbContext is not initialized. Ensure it is properly configured and injected.");
            }

            return context.Products.ToList(); // 从数据库中获取商品
        }

        public static List<Product> GetProducts(string connectionString)
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Description, Price, ImageUrl FROM Products";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }

            return products;
        }
    }
}

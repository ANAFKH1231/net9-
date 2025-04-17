using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;

namespace net9购物网站.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ProductDetailsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Product? Product { get; set; }

        public IActionResult OnGet(int id)
        {
            Console.WriteLine($"Received Product ID: {id}");
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        SELECT Id, Name, Description, Price, ImageUrl, SellerId, BuyerId
                        FROM Products
                        WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Product = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SellerId = reader.GetInt32(5),
                                BuyerId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            };
                        }
                    }
                }

                if (Product == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product details: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;
using System.Security.Claims;

namespace net9购物网站.Pages
{

    public class PaymentModel(IConfiguration configuration) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public Product? Product { get; set; }

        public string? Message { get; set; }

        public void OnGet(int id)
        {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product details: {ex.Message}");
                Message = "加载商品信息失败，请稍后重试。";
            }
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                // Retrieve the logged-in user's ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Message = "无法获取登录用户信息，请重新登录。";
                    return Page();
                }

                var buyerId = int.Parse(userIdClaim.Value);

                // Use raw SQL to update the BuyerId column
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        UPDATE Products
                        SET BuyerId = @BuyerId
                        WHERE Id = @Id";
                    command.Parameters.AddWithValue("@BuyerId", buyerId);
                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Message = "支付成功！";
                        return RedirectToPage("/showgoods");
                    }
                }

                Message = "支付失败，商品不存在。";
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment: {ex.Message}");
                Message = "支付失败，请稍后重试。";
                return Page();
            }
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;
using System.Security.Claims;

namespace net9购物网站.Pages
{
    public class UserDetailModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UserDetailModel(IConfiguration configuration)
        {
            _configuration = configuration;
            User = new User
            {
                Username = string.Empty, // Initialize required property  
                Password = string.Empty  // Initialize required property  
            };
        }

        [BindProperty]
        public new User User { get; set; }

        public List<Product> PurchasedProducts { get; set; } = new List<Product>();
        public List<Product> SoldProducts { get; set; } = new List<Product>();

        public string? Message { get; set; }


        public void OnGet()
        {
            try
            {
                // Get logged-in user's ID  
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Message = "无法获取登录用户信息，请重新登录。";
                    return;
                }

                var userId = int.Parse(userIdClaim.Value);

                // Fetch user data  
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    // Fetch user details  
                    var userCommand = connection.CreateCommand();
                    userCommand.CommandText = "SELECT Id, Username, Password, Email FROM Users WHERE Id = @Id";
                    userCommand.Parameters.AddWithValue("@Id", userId);

                    using (var reader = userCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User = new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }

                    // Fetch purchased products  
                    var purchasedCommand = connection.CreateCommand();
                    purchasedCommand.CommandText = "SELECT Id, Name, Description, Price, ImageUrl, SellerId, BuyerId FROM Products WHERE BuyerId = @BuyerId";
                    purchasedCommand.Parameters.AddWithValue("@BuyerId", userId);

                    using (var reader = purchasedCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PurchasedProducts.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SellerId = reader.GetInt32(5),
                                BuyerId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            });
                        }
                    }

                    // Fetch sold products  
                    var soldCommand = connection.CreateCommand();
                    soldCommand.CommandText = "SELECT Id, Name, Description, Price, ImageUrl, SellerId, BuyerId FROM Products WHERE SellerId = @SellerId";
                    soldCommand.Parameters.AddWithValue("@SellerId", userId);

                    using (var reader = soldCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SoldProducts.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SellerId = reader.GetInt32(5),
                                BuyerId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user details: {ex.Message}");
                Message = "加载用户信息失败，请稍后重试。";
                
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                // Get logged-in user's ID  
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Message = "无法获取登录用户信息，请重新登录。";
                    return Page();
                }

                var userId = int.Parse(userIdClaim.Value);

                // Update user data  
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"  
                               UPDATE Users  
                               SET Username = @Username, Password = @Password, Email = @Email  
                               WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Username", User.Username);
                    command.Parameters.AddWithValue("@Password", User.Password);
                    command.Parameters.AddWithValue("@Email", User.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Id", userId);

                    command.ExecuteNonQuery();
                }

                Message = "用户信息更新成功！";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user details: {ex.Message}");
                Message = "更新用户信息失败，请稍后重试。";
                return Page();
            }
        }
        
    }
}

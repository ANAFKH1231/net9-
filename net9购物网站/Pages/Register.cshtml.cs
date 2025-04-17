using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;
using net9购物网站.Services;
using System.Security.Claims;

namespace net9购物网站.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public new User User { get; set; } = new User
        {
            Username = string.Empty, // Initialize required property  
            Password = string.Empty  // Initialize required property  
        };

        public string? Message { get; set; }

        public Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "注册信息无效，请检查输入。";
                return Task.FromResult<IActionResult>(Page());
            }

            try
            {
                // Ensure connectionString is not null
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    Message = "无法获取数据库连接字符串，请联系管理员。";
                    return Task.FromResult<IActionResult>(Page());
                }

                // Use UserService to register the user
                var success = UserService.Register(User, connectionString);

                if (success)
                {
                    // 跳转到目标页面
                    Console.WriteLine("Registration successful. Redirecting to /PublishProduct."); // Log success
                    return Task.FromResult<IActionResult>(RedirectToPage("/PublishProduct"));
                }
                else
                {
                    Message = "注册失败。";
                    return Task.FromResult<IActionResult>(Page());
                }
            }
            catch (Exception ex)
            {
                Message = "注册失败，请稍后重试。";
                // Log the exception (optional)  
                Console.WriteLine(ex.Message);
                return Task.FromResult<IActionResult>(Page());
            }
        }
    }
}


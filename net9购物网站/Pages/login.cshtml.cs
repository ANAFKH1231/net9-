using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using net9购物网站.MODEL;
using net9购物网站.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace net9购物网站.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? Message { get; set; }
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Message = "数据库连接字符串未配置。";
                return Page();
            }

            var user = await Task.Run(() => UserService.Login(Username, Password, connectionString)); // 模拟异步操作
            if (user != null)
            {
                Console.WriteLine("Login successful. Setting authentication cookie.");
                // 登录成功，设置身份验证 Cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                // 处理 ReturnUrl
                returnUrl ??= Url.Content("~/");
                Console.WriteLine($"Redirecting to {returnUrl}");
                return LocalRedirect(returnUrl);
            }
            else
            {
                Message = "用户名或密码错误。";
                return Page();
            }
        }



    }
}


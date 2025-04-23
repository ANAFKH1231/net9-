using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using net9购物网站.Services;
using System.Security.Claims;

namespace net9购物网站.Pages
{
    public class VerifyEmailModel : PageModel
    {
        [BindProperty]
        public string Code { get; set; } = string.Empty;

        public string? Message { get; set; }
        private readonly IConfiguration _configuration;

        public VerifyEmailModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var expectedCode = TempData["VerificationCode"] as string;
            var email = TempData["Email"] as string;

            if (string.IsNullOrEmpty(email))
            {
                Message = "电子邮件地址无效，请重试。";
                return Page();
            }

            if (expectedCode == Code)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    Message = "数据库连接字符串未配置。";
                    return Page();
                }

                // 从数据库中获取用户信息
                var user = UserService.GetUserByEmail(email, connectionString);
                if (user != null)
                {
                    // 调用 UserService.Login 验证用户
                    var loggedInUser = await Task.Run(() => UserService.Login(user.Username, user.Password, connectionString));
                    if (loggedInUser != null)
                    {
                        // 设置身份验证 Cookie
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, loggedInUser.Username),
                            new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString())
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));


                        return RedirectToPage("/showgoods");
                    }
                    else
                    {
                        Message = "自动登录失败，请重试。";
                        return Page();
                    }
                }
                else
                {
                    Message = "用户信息未找到，请重试。";
                    return Page();
                }
            }
            else
            {
                Message = "验证码错误，请重试。";
                return Page();
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using net9购物网站.Services;
using System.Net.Mail;

namespace net9购物网站.Pages
{
    public class EmailLoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string? Message { get; set; }
        private readonly IConfiguration _configuration;

        public EmailLoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnPost()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Message = "数据库连接字符串未配置。";
                return Page();
            }

            // 检查邮箱是否存在
            var user = UserService.GetUserByEmail(Email, connectionString);
            if (user != null)
            {
                // 发送验证码
                var verificationCode = UserService.SendVerificationCode(Email, _configuration);
                TempData["VerificationCode"] = verificationCode;
                TempData["Email"] = Email;
                return RedirectToPage("/VerifyEmail");
            }
            else
            {
                Message = "该邮箱未注册。";
                return Page();
            }
        }

    }
}


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace net9购物网站.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // 注销用户并清除身份验证 Cookie
            await HttpContext.SignOutAsync("CookieAuth");

            // 重定向到登录页面或其他页面
            return RedirectToPage("/login");
        }
    }
}


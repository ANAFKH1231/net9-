using System.ComponentModel.DataAnnotations;

namespace net9购物网站.MODEL
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "用户名是必填项")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "密码是必填项")]
        public required string Password { get; set; } 

        [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
        public string? Email { get; set; }

        internal object FindFirst(string nameIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}

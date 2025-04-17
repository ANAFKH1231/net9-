using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using net9购物网站.Data;
using net9购物网站.MODEL;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.IO;

namespace net9购物网站.Pages
{
    [Authorize]
    public class PublishProductModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public PublishProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product
        {
            Name = string.Empty, // Initialize required property
            Description = string.Empty // Initialize required property
        };

        public string? Message { get; set; }

        public void OnGet()
        {
            // Initialize any necessary data for the page
        }

        public IActionResult OnPost(IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // 检查是否上传了文件
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    Message = "请上传商品图片。";
                    return Page();
                }

                // 确保 wwwroot/image 文件夹存在
                var imageFolder = Path.Combine("D:\\程序\\net9购物网站\\net9购物网站\\wwwroot\\images");
                if (!Directory.Exists(imageFolder))
                    {
                        Directory.CreateDirectory(imageFolder);
                    }

                // 生成唯一文件名并保存文件
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var filePath = Path.Combine(imageFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                // 生成图片的 URL
                var imageUrl = $"/images/{fileName}";

                // 获取登录用户 ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    {
                        Message = "无法获取登录用户信息，请重新登录。";
                        return Page();
                    }

                // 设置商品信息
                Product.SellerId = int.Parse(userIdClaim.Value);
                Product.BuyerId = null;
                Product.ImageUrl = imageUrl;

                // 使用原生 SQL 插入商品信息
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = @"
                    INSERT INTO Products (Name, Description, Price, ImageUrl, SellerId, BuyerId)
                    VALUES (@Name, @Description, @Price, @ImageUrl, @SellerId, @BuyerId)";
                        command.Parameters.AddWithValue("@Name", Product.Name);
                        command.Parameters.AddWithValue("@Description", Product.Description);
                        command.Parameters.AddWithValue("@Price", Product.Price);
                        command.Parameters.AddWithValue("@ImageUrl", Product.ImageUrl);
                        command.Parameters.AddWithValue("@SellerId", Product.SellerId);
                        command.Parameters.AddWithValue("@BuyerId", DBNull.Value);

                        command.ExecuteNonQuery();
                    }

                Message = "商品发布成功！";
                return RedirectToPage("/showgoods");
            }
            catch (Exception ex)
            {
                Message = "商品发布失败，请稍后重试。";
                Console.WriteLine($"Error: {ex.Message}");
                return Page();
            }
        }
    }
}

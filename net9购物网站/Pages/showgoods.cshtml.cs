using Microsoft.AspNetCore.Mvc.RazorPages;
using net9购物网站.Data;
using net9购物网站.MODEL;

namespace net9购物网站.Pages
{
    public class showgoodsModel : PageModel
    {
        private readonly AppDbContext _context;

        public showgoodsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> Product { get; set; } = new List<Product>();

        public void OnGet()
        {
            // 从数据库中加载 BuyerId 为空的商品
            Product = _context.Products
                              .Where(p => p.BuyerId == null)
                              .ToList();
        }
    }
}


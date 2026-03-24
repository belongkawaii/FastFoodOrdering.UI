using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;

namespace ShopWeb.Pages   // 🔥 nhớ đúng namespace project của bạn
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Product> Products { get; set; } = new();

        public IndexModel()
        {
            _httpClient = new HttpClient();
        }

        /*hàm gọi API lấy dữ liệu sản phẩm*/
        public async Task OnGet() 
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("http://localhost:5014/api/products");
            Products = response ?? new List<Product>();
        }
    }

    public class Product
    {
        public int id { get; set; } 
        public string name { get; set; } = "";
        public decimal price { get; set; }
        public string imageUrl { get; set; } = "";
        
    }
}
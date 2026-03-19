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
            var response = await _httpClient.GetStringAsync("https://fakestoreapi.com/products");

            Products = JsonSerializer.Deserialize<List<Product>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public class Product
    {
        public string title { get; set; } = "";
        public string image { get; set; } = "";
    }
}
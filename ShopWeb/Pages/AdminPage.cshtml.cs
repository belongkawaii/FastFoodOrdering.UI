using Microsoft.AspNetCore.Mvc.RazorPages;
namespace ShopWeb.Pages
{
public class AdminPageModel : PageModel
{
        private readonly HttpClient _httpClient;

        public List<Product> Products { get; set; } = new();

        public AdminPageModel()
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
}
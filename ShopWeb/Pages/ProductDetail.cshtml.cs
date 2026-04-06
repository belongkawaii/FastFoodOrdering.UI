using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

public class ProductDetailModel : PageModel
{
    private readonly HttpClient _httpClient;

    public Product Product { get; set; } = new();
    public List<Product> AllProducts { get; set; } = new();

    public ProductDetailModel()
    {
        _httpClient = new HttpClient();
    }

    public async Task OnGetAsync(int? id)
    {
        // lấy toàn bộ danh sách (cho dropdown)
        var listJson = await _httpClient.GetStringAsync("http://localhost:5014/api/products");
        AllProducts = JsonSerializer.Deserialize<List<Product>>(listJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // lấy product theo id
        if (id == null && AllProducts.Count > 0)
            id = AllProducts[0].id;

        var json = await _httpClient.GetStringAsync($"https://localhost:5001/api/products/{id}");
        Product = JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

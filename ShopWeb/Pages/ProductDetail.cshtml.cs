using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Headers;

namespace ShopWeb.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = "http://localhost:5014/api";

        // Chỉ dùng để hiển thị lên giao diện (Read-only)
        public Product Product { get; set; } = new();
        public List<Product> AllProducts { get; set; } = new();

        public ProductDetailModel()
        {
            _httpClient = new HttpClient();
        }

        // Lấy dữ liệu sản phẩm khi load trang
        public async Task OnGetAsync(int? id)
        {
            try
            {
                var listJson = await _httpClient.GetStringAsync($"{BaseApiUrl}/products");
                AllProducts = JsonSerializer.Deserialize<List<Product>>(listJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                if (id == null && AllProducts.Count > 0) id = AllProducts[0].id;

                var json = await _httpClient.GetStringAsync($"{BaseApiUrl}/products/{id}");
                Product = JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
            }
        }

        // Xử lý lưu dữ liệu (Gọi khi bấm nút Save)
        public async Task<IActionResult> OnPostSaveProductAsync(int id)
        {
            // Bước 1: Lấy dữ liệu trực tiếp từ Request.Form
            var name = Request.Form["ProductName"];
            var priceStr = Request.Form["Product.price"].ToString().Replace(".", "");
                decimal price = decimal.Parse(priceStr);    
            var description = Request.Form["ProductDescription"];
            var imageFile = Request.Form.Files["UploadedImage"];

            // Bước 2: Tạo Multipart content để gửi cho AdminService
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(name.ToString()), "Name");
            content.Add(new StringContent(price.ToString()), "Price");
            content.Add(new StringContent(description.ToString() ?? ""), "Description");

            // Nếu có file ảnh mới thì đóng gói vào Image
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileContent = new StreamContent(imageFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(fileContent, "Image", imageFile.FileName);
            }

            // Bước 3: Đính kèm Authorization Token từ Cookie
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Bước 4: Gọi API PUT đến Backend
            var response = await _httpClient.PutAsync($"{BaseApiUrl}/admin/products/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMsg"] = "Cập nhật sản phẩm thành công!";
                // Thành công thì load lại trang chi tiết sản phẩm đó
                return RedirectToPage(new { id = id });
            }

            // Xử lý khi lỗi (có thể trả về Page kèm lỗi)
            return Page();
        }
    }
}
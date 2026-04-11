using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Headers;

namespace ShopWeb.Pages
{
    public class AddProductAdminModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = "https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api";

        // Khởi tạo đối tượng Product trống để hiển thị form sạch
        public Product Product { get; set; } = new();

        public AddProductAdminModel()
        {
            _httpClient = new HttpClient();
        }

        // Không truy xuất database, để các ô nhập liệu trống hoàn toàn
        public void OnGet()
        {
            Product = new Product();
        }

        public async Task<IActionResult> OnPostSaveProductAsync()
        {
            // Lấy dữ liệu thủ công từ Request.Form
            var name = Request.Form["ProductName"];
            var priceStr = Request.Form["Product.price"].ToString().Replace(".", "");
                decimal price = decimal.Parse(priceStr);    
            var description = Request.Form["ProductDescription"];
            var imageFile = Request.Form.Files["UploadedImage"];

            // KIỂM TRA: Bắt buộc điền full mọi ô và phải có ảnh
            if (string.IsNullOrEmpty(name) || price <= 0 || 
                string.IsNullOrEmpty(description) || imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ thông tin và chọn ảnh sản phẩm.");
                return Page();
            }

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(name.ToString()), "Name");
            content.Add(new StringContent(price.ToString()), "Price");
            content.Add(new StringContent(description.ToString()), "Description");

            // Đóng gói file ảnh
            var fileContent = new StreamContent(imageFile.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "Image", imageFile.FileName);

            // Authorization: Lấy token từ Cookie
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Gọi API POST để tạo sản phẩm mới (AdminService.CreateProductAsync)
            var response = await _httpClient.PostAsync($"{BaseApiUrl}/admin/products", content);

            if (response.IsSuccessStatusCode)
            {
                // Sau khi thêm thành công, quay về trang danh sách Admin
                TempData["SuccessMsg"] = "Thêm sản phẩm thành công!";
                return RedirectToPage("/AdminPage");
            }
            else
            {
                Console.WriteLine("Error adding product: " + response.StatusCode);
                // Nếu có lỗi, hiển thị thông báo lỗi từ Server (nếu có)
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi từ Server: {errorContent}");
            }

            return Page();
        }
    }
}
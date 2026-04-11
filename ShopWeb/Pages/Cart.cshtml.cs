using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace ShopWeb.Pages
{
    public class CartModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public Cart Carts { get; set; } = new();

        public CartModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        private void AttachToken()
        {
            var token = Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Hàm lấy giỏ hàng lúc mới vào trang
        public async Task OnGetAsync()
        {
            AttachToken();
            try
            {
                var response = await _httpClient.GetAsync("https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/cart");
                if (response.IsSuccessStatusCode)
                {
                    Carts = await response.Content.ReadFromJsonAsync<Cart>() ?? new Cart();
                }
            }
            catch { Carts = new Cart(); }
        }

        // 🔥 HÀM XỬ LÝ AJAX TĂNG SỐ LƯỢNG
        public async Task<JsonResult> OnPostIncreaseAsync(int itemId, int productId)
        {
            AttachToken();
            var res = await _httpClient.PutAsync($"https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/cart/increase/{itemId}", null);
            return await GetUpdatedCartData(itemId, res.IsSuccessStatusCode);
        }

        // 🔥 HÀM XỬ LÝ AJAX GIẢM SỐ LƯỢNG
        public async Task<JsonResult> OnPostDecreaseAsync(int itemId, int productId)
        {
            AttachToken();
            var res = await _httpClient.PutAsync($"https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/cart/decrease/{itemId}", null);
            return await GetUpdatedCartData(itemId, res.IsSuccessStatusCode);
        }

        // 🔥 HÀM XỬ LÝ AJAX XÓA MÓN ĂN
        public async Task<JsonResult> OnPostRemoveAsync(int itemId, int productId)
        {
            AttachToken();
            var res = await _httpClient.DeleteAsync($"https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/cart/remove/{itemId}");
            return await GetUpdatedCartData(itemId, res.IsSuccessStatusCode);
        }

        // 🛠 HÀM HỖ TRỢ: Lấy giỏ hàng từ DB ra để tính toán số liệu chuẩn 100%
        private async Task<JsonResult> GetUpdatedCartData(int itemId, bool isSuccess)
        {
            if (!isSuccess) return new JsonResult(new { success = false, message = "Lỗi khi cập nhật!" });

            // Gọi API lấy lại toàn bộ giỏ hàng mới nhất
            var cartResponse = await _httpClient.GetAsync("https://fastfoodorderingsystem-gaeka7bbhncrhnhp.southeastasia-01.azurewebsites.net/api/cart");
            if (cartResponse.IsSuccessStatusCode)
            {
                var cart = await cartResponse.Content.ReadFromJsonAsync<Cart>();
                if (cart == null || cart.items == null || !cart.items.Any()) 
                {
                    // Giỏ hàng trống
                    return new JsonResult(new { success = true, cartTotalQty = 0, cartTotalAmount = "0" });
                }

                // Tìm món ăn vừa được cập nhật để trả về số lượng và tổng tiền của riêng nó
                var updatedItem = cart.items.FirstOrDefault(i => i.id == itemId);
                
                return new JsonResult(new
                {
                    success = true,
                    newQuantity = updatedItem?.quantity ?? 0,
                    itemTotal = ((updatedItem?.quantity ?? 0) * (updatedItem?.product?.price ?? 0)).ToString("N0"),
                    cartTotalQty = cart.TotalQuantity,
                    cartTotalAmount = cart.TotalAmount.ToString("N0")
                });
            }
            
            return new JsonResult(new { success = false, message = "Không tải được dữ liệu mới." });
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

public class CheckoutModel : PageModel
{
    private readonly HttpClient _httpClient = new HttpClient();

    public Cart Cart { get; set; } = new();

    [BindProperty]
    public OrderRequest Order { get; set; } = new();

    public string Message { get; set; }

    public async Task OnGetAsync()
    {
        var token = Request.Cookies["JWToken"];

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync("https://localhost:7214/api/cart");

            if (res.IsSuccessStatusCode)
            {
                Cart = await res.Content.ReadFromJsonAsync<Cart>() ?? new Cart();
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = Request.Cookies["JWToken"];

        if (Cart.items == null || !Cart.items.Any())
        {
            Message = "Giỏ hàng trống!";
            return Page();
        }

        Order.Items = Cart.items.Select(i => new OrderItem
        {
            ProductId = i.productId,
            Quantity = i.quantity
        }).ToList();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var res = await _httpClient.PostAsJsonAsync(
            "https://localhost:7214/api/orders",
            Order
        );

        if (res.IsSuccessStatusCode)
{
    Message = "✅ Đặt hàng thành công!";

    //API XÓA GIỎ HÀNG
    await client.DeleteAsync("https://localhost:7214/api/cart");

    //reload lại giỏ hàng
    Carts.items = new List<CartItemDto>();
    Carts.TotalQuantity = 0;
    Carts.TotalAmount = 0;
}

        Message = "Đặt hàng thất bại!";
        return Page();
    }
}
public class OrderRequest
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Note { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

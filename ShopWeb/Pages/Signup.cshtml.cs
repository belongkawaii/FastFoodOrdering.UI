using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SignupModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (Password != ConfirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu không khớp");
            return Page();
        }

        // lưu database sau
        return RedirectToPage("/Signin");
    }
}
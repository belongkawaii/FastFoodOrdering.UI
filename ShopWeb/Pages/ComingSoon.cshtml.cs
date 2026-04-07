using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShopWeb.Pages
{
    public class ComingSoonModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? FeatureName { get; set; }

        public void OnGet()
        {
            
        }
    }
}
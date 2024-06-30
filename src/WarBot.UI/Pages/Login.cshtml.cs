using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WarBot.UI.Pages
{
    public class LoginModel : PageModel
    {
        [Inject]
        IAuthenticationHandler authenticationHandler { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.ChallengeAsync(new AuthenticationProperties
            {
                RedirectUri = "/index"
            });
            //await authenticationHandler.AuthenticateAsync();
            //return LocalRedirect("/index");
            return Page();
        }
    }
}

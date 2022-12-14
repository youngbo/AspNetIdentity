using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;
using Web_App.Authorization;

namespace AspNetIdentity.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //verify the credential
            if (Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@website.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", ""),
                    new Claim("Manager", ""),
                    new Claim("EmploymentDate","2022-6-01")
                };

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };


                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(new List<ClaimsIdentity> { identity });

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    principal, authProperties);
                return RedirectToPage("/index");
            }
            return Page();
        }
    }
}

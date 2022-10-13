using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AspNetCoreIdentity.Services;
using AspNetCoreIdentity.Data.Account;
using System.Security.Claims;

namespace AspNetCoreIdentity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validating Email address (Optional)

            // Create the user 
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                Department = RegisterViewModel.Department,
                Position = RegisterViewModel.Position
            };

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if (result.Succeeded)
            {
                var claimOrganisation = new Claim("Organisation", RegisterViewModel.Organisation);
                await this.userManager.AddClaimAsync(user, claimOrganisation);

                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

                // For now, it redirect to confirmEmail link.
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                            values: new { userId = user.Id, token = confirmationToken }));

                // It requires SMTP configuration in an appsettings.json
                //var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //    values: new { userId = user.Id, token = confirmationToken });

                //await emailService.SendAsync("frankliu.associates@gmail.com",
                //    user.Email,
                //    "Please confirm your email",
                //    $"Please click on this link to confirm your email address: {confirmationLink}");

                //return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Organisation { get; set; }
    }
}


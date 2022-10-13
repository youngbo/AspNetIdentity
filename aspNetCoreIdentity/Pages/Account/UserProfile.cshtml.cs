using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreIdentity.Data.Account;

namespace AspNetCoreIdentity.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.UserProfile = new UserProfileViewModel();
            this.SuccessMessage = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            this.SuccessMessage = string.Empty;
            var (user, organisationClaim) = await GetUserInfoAsync();
            this.UserProfile.Email = User.Identity.Name;
            this.UserProfile.Organisation = organisationClaim?.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SuccessMessage");
            if (!ModelState.IsValid) return Page();

            try
            {
                var (user, organisationClaim) = await GetUserInfoAsync();
                if (organisationClaim!= null)
                {
                    await userManager.ReplaceClaimAsync(user, organisationClaim, 
                        new Claim(organisationClaim.Type, UserProfile.Organisation));
                }
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured when saving user profile.");
            }

            this.SuccessMessage = "The user profile is saved successfully.";

            return Page();
        }

        private async Task<(Data.Account.User, Claim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var claims = await userManager.GetClaimsAsync(user);
            var organisationClaim = claims.FirstOrDefault(x => x.Type == "Organisation");

            return (user, organisationClaim);
        }
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; }

        [Required]
        public string Organisation { get; set; }
    }
}

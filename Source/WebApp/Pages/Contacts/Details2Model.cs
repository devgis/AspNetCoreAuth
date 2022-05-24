using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Authorization;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Contacts
{
    [AllowAnonymous]
    public class Details2Model : DI_BasePageModel
    {
        public Details2Model(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contact? _contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (_contact == null)
            {
                return NotFound();
            }
            Contact = _contact;

            if (!User.Identity!.IsAuthenticated)
            {
                return Challenge();
            }

            var isAuthorized = User.IsInRole(Constants.WebappsRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Contact.OwnerID
                && Contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }
    }
}

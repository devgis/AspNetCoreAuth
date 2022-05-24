using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Authorization;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Contacts
{
    public class DetailsModel : DI_BasePageModel
    {
        public DetailsModel(
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

        public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
        {
            var contact = await Context.Contact.FirstOrDefaultAsync(
                                                      m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ContactStatus.Approved)
                                                       ? ContactOperations.Approve
                                                       : ContactOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            contact.Status = status;
            Context.Contact.Update(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

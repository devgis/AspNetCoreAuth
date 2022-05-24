using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Authorization;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Contacts
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contact? _contact = await Context.Contact.FirstOrDefaultAsync(
                                                 m => m.ContactId == id);

            if (_contact == null)
            {
                return NotFound();
            }
            Contact = _contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Contact,
                                                     ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var contact = await Context
                .Contact.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, contact,
                                                     ContactOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Contact.Remove(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

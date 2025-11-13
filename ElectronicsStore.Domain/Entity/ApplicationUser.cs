using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace ElectronicsStore.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}

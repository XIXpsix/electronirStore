using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace ElectronicsStore.Domain.Entity
{
    public class ApplicationUser : global::Microsoft.AspNetCore.Identity.IdentityUser
    {
        public required string FirstName { get; set; } 
        public required string LastName { get; set; } 
        
    }
}

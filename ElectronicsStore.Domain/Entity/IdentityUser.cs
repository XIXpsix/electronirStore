namespace ElectronicsStore.Domain.Entity
{
    public class IdentityUser
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
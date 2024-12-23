namespace Picus.Api.Models;

public class League : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int AdminUserId { get; set; }
    
    // Navigation properties
    public User AdminUser { get; set; } = null!;
    public ICollection<User> Members { get; set; } = new List<User>();
}

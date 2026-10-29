using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared.Authentification;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Login { get; set; } = string.Empty;

    public RoleEnum Role { get; set; }
}

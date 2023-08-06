using System.ComponentModel.DataAnnotations;

namespace UserManagement.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }
    public Guid PasswordId { get; set; }
    public Password Password { get; set; }
}

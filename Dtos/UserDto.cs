namespace UserManagement.Dtos
{
    public class UserDto
    {
        public string? Email { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
    }
    public record UserRec(string email, string username, string password);
    public record LoginRec(string username, string password);
}

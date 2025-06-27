namespace FullStackApp.Server.Models.User
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
        public UserDTO(string username, IList<string> roles, string id)
        {
            UserId = id;
            Username = username;
            Roles = roles;
        }
    }
}

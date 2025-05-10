namespace Siska.Admin.Model.DTO.System
{
    public class UserSignInDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<string>? Roles { get; set; }
        public string Token { get; set; }
    }
}

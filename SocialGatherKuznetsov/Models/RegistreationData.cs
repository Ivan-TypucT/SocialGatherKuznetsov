using System.ComponentModel.DataAnnotations;

namespace SocialGatherKuznetsov.Models
{
    public class RegistreationData
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string? UserId { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string token { get; set; }


        public RegistreationData()
        {
            Email = "";
            Login = "";
            Password = "";
            token = "";
            UserId = null;
        }

    }

}

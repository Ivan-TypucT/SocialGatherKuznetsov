using System.ComponentModel.DataAnnotations;

namespace SocialGatherKuznetsov.Models
{
    public class RegistreationData
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string? UserId { get; set; }

        public string Login { get; set; }

        public byte[]? Password { get; set; }

        public byte[]? salt { get; set; }

        public string token { get; set; }

        public bool Subscribed { get; set; }


        public RegistreationData()
        {
            Email = "";
            Login = "";
            Password = null;
            salt = null;
            token = "";
            UserId = null;
        }

    }

}

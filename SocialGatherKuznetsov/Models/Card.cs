using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SocialGatherKuznetsov.Models
{
    public class Card
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? Name { get; set; }

        public int PictureNum { get; set; }

        public string? Text { get; set; }

        public string? Place { get; set; }

        public DateTime? Date { get; set; }

        public int GuestsNumMax { get; set; }

        public int GuestsNumCurrent { get; set; }

        [Required]
        [DataMember]
        public virtual ICollection<Guest> GuestsList { get; set; }

        //Тэги это буквы в строке, 5 букв - пять тэгов
        public string? Tags { get; set; }



        public Card()
        {
            Name = "";
            Text = "";
            Date = null;
            GuestsList = new List<Guest>();
 
        }


    }
    public class Guest
    {
        public int Id { get; set; }
        public string UserId { get; set; }

    }

}
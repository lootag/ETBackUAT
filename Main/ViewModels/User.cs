using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Main.ViewModels
{
    public class User
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Status Status {get; set; }
    }
}
using System;
using Models.Enums;

namespace Models
{
    public class User
    {
        public User( string Email, string Password, Status Status)
        {
            this.Email = Email;
            this.Password = Password;
            this.Status = Status;
        }

        public string Email { get; }
        public string Password { get; }
        public Status Status {get; }
    }

}

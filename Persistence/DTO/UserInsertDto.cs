using Models.Enums;

namespace Persistence.DTO
{
    public class UserInsertDto
    {
        public UserInsertDto(string Email, byte[] Hash, byte[] Salt, Status Status)
        {
            this.Email = Email;
            this.Hash = Hash;
            this.Salt = Salt;
            this.Status = Status;
        }

        public string Email { get; }
        public byte[] Hash { get; }
        public byte[] Salt { get; }
        public Status Status {get; }
    }
}
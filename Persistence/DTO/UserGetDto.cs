using Models.Enums;

namespace Persistence.DTO
{
    public class UserGetDto
    {
        public UserGetDto(long Id, string Email, byte[] Hash, byte[] Salt, int Status)
        {
            this.Id = Id;
            this.Email = Email;
            this.Hash = Hash;
            this.Salt = Salt;
            this.Status = Status;
        }

        public long Id { get; }
        public string Email { get; }
        public byte[] Hash { get; }
        public byte[] Salt { get; }
        public int Status {get; }
    }
}
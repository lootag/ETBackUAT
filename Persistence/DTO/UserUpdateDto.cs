using Models.Enums;

namespace Persistence.DTO
{
    public class UserUpdateDto
    {
        public UserUpdateDto(long Id, Status Status)
        {
            this.Id = Id;
            this.Status = Status;
        }

        public long Id { get; }
        public Status Status { get; set; }
    }
}
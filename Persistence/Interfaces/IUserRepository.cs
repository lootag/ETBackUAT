using System.Collections.Generic;
using Persistence.DTO;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {
        void Insert(UserInsertDto dto, IContext context);
        void Update(UserUpdateDto dto, IContext context);
        IList<UserGetDto> GetByEmail(string Email, IContext context);
    }
}
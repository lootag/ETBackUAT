using System;
using System.Collections.Generic;
using Persistence.Interfaces;
using Persistence.DTO;

namespace Persistence
{
    public class UserRepository : IUserRepository
    {
        public void Insert(UserInsertDto dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));
            const string sql = @"insert into dbo.Users (Email, Hash, Salt, Status)
                                values(@Email, @Hash, @Salt, @Status)";
            context.Execute(sql, new
            {
                dto.Email,
                dto.Hash,
                dto.Salt,
                dto.Status
            });
        }

        public void Update(UserUpdateDto dto, IContext context)
        {
            if(dto == null) throw new ArgumentNullException(nameof(dto));
            if(context == null) throw new ArgumentNullException(nameof(context));
            const string sql = @"update dbo.Users set Status = @Status where id = @id";
            context.Execute(sql, new
            {
                dto.Id,
                dto.Status
            });
        }

        public IList<UserGetDto> GetByEmail(string email, IContext context)
        {
            IList<UserGetDto> toReturn = new List<UserGetDto>();
            const string sql = @"select * from dbo.Users where Email = @Email";
            toReturn = context.Query<UserGetDto>(sql, new
            {
                Email = email
            });

            return toReturn;
        }
    }
}
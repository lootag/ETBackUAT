using System;
using System.Collections.Generic;
using Models.Enums;
using Persistence;
using Persistence.Interfaces;
using Persistence.DTO;
using Xunit;
using Xunit.Abstractions;

namespace UnitTesting
{
    public class UserRepositoryTest : IDisposable
    {
        //Arrange
        private readonly IContextFactory contextFactory;
        private readonly UserRepository repository;
        public UserRepositoryTest()
        {
            string connectionString = "Server=tcp:edgartools.database.windows.net,1433;Initial Catalog=edgartoolsDev;Persist Security Info=False;User ID=edgartools;Password=etphonehome01!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            contextFactory = new DapperContextFactory(connectionString);
            repository = new UserRepository();

        }

        public void Dispose()
        {
            using(IContext context = contextFactory.Create())
            {
                const string sql = "delete from dbo.Users";
                context.Execute(sql);
                context.Commit();
            }
        }

        [Fact]
        public void Should_User_Be_Inserted()
        {
            //Arrange
            string Email = "test.user@test.com";
            byte[] Hash = {1,2,3};
            byte[] Salt = {4,5,6};
            UserInsertDto dto = new UserInsertDto(Email, Hash, Salt, Status.Free);
            //Act
            using(IContext context = contextFactory.Create())
            {
                repository.Insert(dto, context);
                context.Commit();
            }
            
            using(IContext context = contextFactory.Create())
            {
                const string sql = "select * from dbo.Users";
                IList<dynamic> users = new List<dynamic>();
                users = context.Query<dynamic>(sql);
                
                //Assert
                Assert.Equal(1, users.Count);
                Assert.Equal(Email, users[0].Email);
                Assert.Equal(Hash, users[0].Hash);
                Assert.Equal(Salt, users[0].Salt);
                Assert.Equal((int)Status.Free, (int)users[0].Status);
            }
            
        }

        [Fact]
        public void Should_Status_Be_Updated()
        {
            //Arrange
            const string sqlInsertUser = @"insert into dbo.Users(Email, Hash, Salt, Status) 
                                            values(@Email, @Hash, @Salt, @Status)";            
            const string sqlRetrieveId = "select Id from dbo.Users where Email = @Email";
            const string sqlRetriveStatus = "select Status from dbo.Users where Id = @Id";
            string Email = "test.email@test.com";
            byte[] Hash = {1, 2, 3};
            byte[] Salt = {4, 5, 6};
            using(IContext context = contextFactory.Create())
            {
                //Arrange
                context.Execute(sqlInsertUser, new
                {
                    Email,
                    Hash,
                    Salt,
                    Status = Status.Free
                });
                IList<long> idList = context.Query<long>(sqlRetrieveId, new
                {
                    Email = Email
                });

                long Id = idList[0];
                //Act
                UserUpdateDto updateDto = new UserUpdateDto(Id, Status.Premium);
                repository.Update(updateDto, context);
                context.Commit();
                IList<Status> newStatusList = context.Query<Status>(sqlRetriveStatus, new
                {
                    Id = Id
                });
                int newStatus = (int)newStatusList[0];
                //Assert
                Assert.Equal((int)Status.Premium, newStatus);
            }
        }

        [Fact]
        public void Should_Return_Correct_Records()
        {
            string Email = "test.email@test.com";
            byte[] Hash = {1, 2, 3};
            byte[] Salt = {1, 2, 3};
            using(IContext context = contextFactory.Create())
            {
                const string sqlInsertUser = "insert into dbo.Users(Email, Hash, Salt, Status) values(@Email, @Hash, @Salt, @Status)";
                context.Execute(sqlInsertUser, new
                {
                    Email = Email,
                    Hash = Hash,
                    Salt = Salt,
                    Status = Status.Free
                });
                context.Commit();
                IList<UserGetDto> userList = repository.GetByEmail(Email, context);
                Assert.Equal(1, userList.Count);
                Assert.Equal(Email, userList[0].Email);
                Assert.Equal(Hash, userList[0].Hash);
                Assert.Equal(Salt, userList[0].Salt);
                Assert.Equal((int)Status.Free, userList[0].Status);
            }            

        }
    }
}

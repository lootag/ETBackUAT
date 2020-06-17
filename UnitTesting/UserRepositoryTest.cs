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
        public void Should_Record_Be_Inserted()
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
    }
}

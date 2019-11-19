using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Data;
using TicTacToe.Models;
using TicTacToe.Services;
using Xunit;

namespace TicTacToe.UnitTests
{
    public class UserServiceTests
    {
        [Theory]
        [InlineData("test@test.com", "test", "test", "test123!")]
        [InlineData("test1@test.com", "test1", "test1", "test123!")]
        [InlineData("test2@test.com", "test2", "test2", "test123!")]
        public async Task ShouldAddUser(string email, string firstName, string lastName, string password)
        {
            var userModel = new UserModel
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password
            };

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<GameDbContext>()
                .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=TicTacToe;Trusted_Connection=True;MultipleActiveResultSets=true");
            var userService = new UserService(dbContextOptionsBuilder.Options);

            var userAdded = await userService.RegisterUser(userModel);
            Assert.True(userAdded);
        }
    }
}

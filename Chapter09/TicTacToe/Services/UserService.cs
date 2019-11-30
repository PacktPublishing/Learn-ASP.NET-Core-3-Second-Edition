using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public class UserService : IUserService
    {
        private static ConcurrentBag<UserModel> _userStore;
        private DbContextOptions<GameDbContext> _dbContextOptions;
        public UserService(DbContextOptions<GameDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        static UserService()
        {
            _userStore = new ConcurrentBag<UserModel>();
        }

        public UserService()
        {
        }

        //public Task<bool> RegisterUser(UserModel userModel)
        //{
        //    _userStore.Add(userModel);
        //    return Task.FromResult(true);
        //}
        public async Task<bool> RegisterUser(UserModel userModel)
        {
            using (var db = new GameDbContext(_dbContextOptions))
            {
                db.UserModels.Add(userModel);
                await db.SaveChangesAsync();
                return true;
            }
        }
        public Task<bool> IsOnline(string name)
        {
            return Task.FromResult(true);
        }

        //public Task<UserModel> GetUserByEmail(string email)
        //{
        //    return Task.FromResult(_userStore.FirstOrDefault(
        //     u => u.Email == email));
        //}
        public async Task<UserModel> GetUserByEmail(string email)
        {
            using (var db = new GameDbContext(_dbContextOptions))
            {
                return await db.UserModels.FirstOrDefaultAsync(
                 x => x.Email == email);
            }
        }

        public async Task UpdateUser(UserModel userModel)
        {
            using (var gameDbContext =
              new GameDbContext(_dbContextOptions))
            {
                gameDbContext.Update(userModel);
                await gameDbContext.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<UserModel>> GetTopUsers(int numberOfUsers)
        {
            using (var gameDbContext =  new GameDbContext(_dbContextOptions))
            {
                return await gameDbContext.UserModels?.OrderByDescending(
                 x => x.Score).ToListAsync();
            }
        }
        public async Task<bool> IsUserExisting(string email)
        {
            using (var gameDbContext =
             new GameDbContext(_dbContextOptions))
            {
                return await gameDbContext.UserModels.AnyAsync(
                 user => user.Email == email);
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public interface IUserService
    {
        Task<bool> IsOnline(string name);
        Task<bool> RegisterUser(UserModel userModel);
        Task<UserModel> GetUserByEmail(string email);
        Task UpdateUser(UserModel user);
        Task<IEnumerable<UserModel>> GetTopUsers(int numberOfUsers);
        Task<bool> ConfirmEmail(string email, string code);
        Task<string> GetEmailConfirmationCode(UserModel user);
    }
}
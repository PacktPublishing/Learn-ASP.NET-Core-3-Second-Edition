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
    }
}
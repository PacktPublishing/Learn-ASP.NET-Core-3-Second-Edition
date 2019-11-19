using System.Threading.Tasks;

namespace TicTacToe.Services
{
    public interface IEmailService
    {
        Task SendEmail(string emailTo, string subject, string message);
    }
}
using System.Threading.Tasks;

namespace TicTacToe.Services
{
    public interface IEmailTemplateRenderService
    {
        Task<string> RenderTemplate<T>(string templateName, T model, string host) where T : class;
    }
}
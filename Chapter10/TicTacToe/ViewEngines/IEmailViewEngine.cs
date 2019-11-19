using System.Threading.Tasks;

namespace TicTacToe.ViewEngines
{
    public interface IEmailViewEngine
    {
        Task<string> RenderEmailToString<TModel>(string viewName, TModel model);
    }
}
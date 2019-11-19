using System;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public interface IGameInvitationService
    {
        Task<GameInvitationModel> Add(GameInvitationModel gameInvitationModel);
        Task<GameInvitationModel> Get(Guid id);
        Task Update(GameInvitationModel gameInvitationModel);
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Extensions;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public class GameSessionService : IGameSessionService
    {
        private static ConcurrentBag<GameSessionModel> _sessions;
        private IUserService _UserService;
        public GameSessionService(IUserService userService)
        {
            _UserService = userService;
        }

        static GameSessionService()
        {
            _sessions = new ConcurrentBag<GameSessionModel>();
        }

        public Task<GameSessionModel> GetGameSession(Guid gameSessionId)
        {
            return Task.Run(() => _sessions.FirstOrDefault(x => x.Id == gameSessionId));
        }

        public async Task<GameSessionModel> CreateGameSession(Guid invitationId, string invitedByEmail, string invitedPlayerEmail)
        {
            var invitedBy = await _UserService.GetUserByEmail(invitedByEmail);
            var invitedPlayer = await _UserService.GetUserByEmail(invitedPlayerEmail);

            GameSessionModel session = new GameSessionModel
            {
                User1 = invitedBy,
                User2 = invitedPlayer,
                Id = invitationId,
                ActiveUser = invitedBy
            };

            _sessions.Add(session);
            return session;
        }

        public async Task<GameSessionModel> AddTurn(Guid id, string email, int x, int y)
        {
            var gameSession = _sessions.FirstOrDefault(session => session.Id == id);
            List<TurnModel> turns;
            if (gameSession.Turns.IsNullOrEmpty())
                turns = new List<TurnModel>(gameSession.Turns);
            else
                turns = new List<TurnModel>();

            turns.Add(new TurnModel {
                User = await _UserService.GetUserByEmail(email), X = x,  Y = y  });

            if (gameSession.User1?.Email == email)
                gameSession.ActiveUser = gameSession.User2;
            else
                gameSession.ActiveUser = gameSession.User1;

            gameSession.TurnFinished = true;
            _sessions = new ConcurrentBag<GameSessionModel>(_sessions.Where(u => u.Id != id))
                {  gameSession };
            return gameSession;
        }
    }
}

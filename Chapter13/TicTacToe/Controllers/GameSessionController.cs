using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public class GameSessionController : Controller
    {
        private IGameSessionService _gameSessionService;
        public GameSessionController(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var session = await _gameSessionService.GetGameSession(id);
            var userService = HttpContext.RequestServices.GetService<IUserService>();

            if (session == null)
            {
                var gameInvitationService = Request.HttpContext.RequestServices.GetService<IGameInvitationService>();
                var invitation = await gameInvitationService.Get(id);

                var invitedPlayer = await userService.GetUserByEmail(invitation.EmailTo);
                var invitedBy = await userService.GetUserByEmail(invitation.InvitedBy);

                session = await _gameSessionService.CreateGameSession(invitation.Id, invitedBy, invitedPlayer);
            }
            return View(session);
        }

        [Produces("application/json")]
        [HttpPost("/restapi/v1/SetGamePosition/{sessionId}")]
        public async Task<IActionResult> SetPosition([FromRoute]Guid sessionId)
        {
            if (sessionId != Guid.Empty)
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var bodyString = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(bodyString))
                        return BadRequest("Body is empty");

                    var turn = JsonConvert.DeserializeObject<TurnModel>(bodyString);

                    turn.User = await HttpContext.RequestServices.GetService<IUserService>().GetUserByEmail(turn.Email);
                    turn.UserId = turn.User.Id;
                    if (turn == null)
                        return BadRequest("You must pass a TurnModel object in your body");

                    var gameSession = await _gameSessionService.GetGameSession(sessionId);

                    if (gameSession == null)
                        return BadRequest($"Cannot find Game Session {sessionId}");

                    if (gameSession.ActiveUser.Email != turn.User.Email)
                        return BadRequest($"{turn.User.Email} cannot play this turn");

                    gameSession = await _gameSessionService.AddTurn(gameSession.Id, turn.User, turn.X, turn.Y);
                    if (gameSession != null && gameSession.ActiveUser.Email != turn.User.Email)
                        return Ok(gameSession);
                    else
                        return BadRequest("Cannot save turn");
                }
            }
            return BadRequest("Id is empty");
        }

        [Produces("application/json")]
        [HttpGet("/restapi/v1/GetGameSession/{sessionId}")]
        public async Task<IActionResult> GetGameSession(Guid sessionId)
        {
            if (sessionId != Guid.Empty)
            {
                var session = await _gameSessionService.GetGameSession(sessionId);
                if (session != null)
                {
                    return Ok(session);
                }
                else
                {
                    return NotFound($"can not found session {sessionId}");
                }
            }
            else
            {
                return BadRequest("session id is null");
            }
        }

        [HttpGet("/restapi/v1/CheckGameSessionIsFinished/{sessionId}")]
        public async Task<IActionResult> CheckGameSessionIsFinished(Guid sessionId)
        {
            if (sessionId != Guid.Empty)
            {
                var session = await _gameSessionService.GetGameSession(sessionId);
                if (session != null)
                {
                    if (session.Turns.Count() == 9)
                        return Ok("The game was a draw.");

                    var userTurns = session.Turns.Where(x => x.User.Id == session.User1.Id).ToList();
                    var user1Won = CheckIfUserHasWon(session.User1?.Email, userTurns);
                    if (user1Won)
                    {
                        return Ok($"{session.User1.Email} has won the game.");
                    }
                    else
                    {
                        userTurns = session.Turns.Where(x => x.User.Id == session.User2.Id).ToList();
                        var user2Won = CheckIfUserHasWon(session.User2?.Email, userTurns);

                        if (user2Won)
                            return Ok($"{session.User2.Email} has won the game.");
                        else
                            return Ok("");
                    }
                }
                else
                {
                    return NotFound($"Cannot find session {sessionId}.");
                }
            }
            else
            {
                return BadRequest("SessionId is null.");
            }
        }

        private bool CheckIfUserHasWon(string email, List<TurnModel> userTurns)
        {
            if (userTurns.Any(x => x.X == 0 && x.Y == 0) && userTurns.Any(x => x.X == 1 && x.Y == 0) && userTurns.Any(x => x.X == 2 && x.Y == 0))
                return true;
            else if (userTurns.Any(x => x.X == 0 && x.Y == 1) && userTurns.Any(x => x.X == 1 && x.Y == 1) && userTurns.Any(x => x.X == 2 && x.Y == 1))
                return true;
            else if (userTurns.Any(x => x.X == 0 && x.Y == 2) && userTurns.Any(x => x.X == 1 && x.Y == 2) && userTurns.Any(x => x.X == 2 && x.Y == 2))
                return true;
            else if (userTurns.Any(x => x.X == 0 && x.Y == 0) && userTurns.Any(x => x.X == 0 && x.Y == 1) && userTurns.Any(x => x.X == 0 && x.Y == 2))
                return true;
            else if (userTurns.Any(x => x.X == 1 && x.Y == 0) && userTurns.Any(x => x.X == 1 && x.Y == 1) && userTurns.Any(x => x.X == 1 && x.Y == 2))
                return true;
            else if (userTurns.Any(x => x.X == 2 && x.Y == 0) && userTurns.Any(x => x.X == 2 && x.Y == 1) && userTurns.Any(x => x.X == 2 && x.Y == 2))
                return true;
            else if (userTurns.Any(x => x.X == 0 && x.Y == 0) && userTurns.Any(x => x.X == 1 && x.Y == 1) && userTurns.Any(x => x.X == 2 && x.Y == 2))
                return true;
            else if (userTurns.Any(x => x.X == 2 && x.Y == 0) && userTurns.Any(x => x.X == 1 && x.Y == 1) && userTurns.Any(x => x.X == 0 && x.Y == 2))
                return true;
            else
                return false;
        }
    }
}

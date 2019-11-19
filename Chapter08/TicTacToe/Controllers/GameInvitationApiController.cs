using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    [Produces("application/json")]
    [Route("restapi/v1/GameInvitation")]
    public class GameInvitationApiController : Controller
    {
        private IGameInvitationService _gameInvitationService;
        private IUserService _userService;
        public GameInvitationApiController(IGameInvitationService gameInvitationService, IUserService userService)
        {
            _gameInvitationService = gameInvitationService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<GameInvitationModel>> Get()
        {
            return await _gameInvitationService.All();
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<GameInvitationModel> Get(Guid id)
        {
            return await _gameInvitationService.Get(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]GameInvitationModel  invitation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invitedPlayer = _userService.GetUserByEmail(invitation.EmailTo);
            if (invitedPlayer == null) return BadRequest();

            _gameInvitationService.Add(invitation);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]GameInvitationModel invitation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invitedPlayer =   _userService.GetUserByEmail(invitation.EmailTo);
            if (invitedPlayer == null) return BadRequest();

            _gameInvitationService.Update(invitation);
            return Ok();
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _gameInvitationService.Delete(id);
        }
    }
}

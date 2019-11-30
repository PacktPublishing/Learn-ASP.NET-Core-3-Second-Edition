using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Halcyon.Web.HAL;
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
        public async Task<IActionResult> Get()
        {
            var invitations = await _gameInvitationService.All();
            var responseConfig = new HALModelConfig
            {
                LinkBase = $"{Request.Scheme}://{Request.Host.ToString()}",
                ForceHAL = Request.ContentType == "application/hal+json" ? true : false
            };

            var response = new HALResponse(responseConfig);
            response.AddLinks(new Link("self", "/GameInvitation"),
                new Link("confirm", "/GameInvitation/{id}/Confirm"));

            List<HALResponse> invitationsResponses = new List<HALResponse>();
            foreach (var invitation in invitations)
            {
                var rInv = new HALResponse(invitation, responseConfig);

                rInv.AddLinks(new Link("self", "/GameInvitation/" + invitation.Id));
                rInv.AddLinks(new Link("confirm", $"/GameInvitation/{invitation.Id}/confirm"));

                var invitedPlayer = _userService.GetUserByEmail(invitation.EmailTo);
                rInv.AddEmbeddedResource("invitedPlayer", invitedPlayer, new Link[]
                {
            new Link("self", $"/User/{invitedPlayer.Id}")
                });

                var invitedBy = _userService.GetUserByEmail(invitation.InvitedBy);
                rInv.AddEmbeddedResource("invitedBy", invitedBy, new Link[]
                {
            new Link("self", $"/User/{invitedBy.Id}")
                });

                invitationsResponses.Add(rInv);
            }

            response.AddEmbeddedCollection("invitations", invitationsResponses);
            return this.HAL(response);
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<GameInvitationModel> Get(Guid id)
        {
            return await _gameInvitationService.Get(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]GameInvitationModel invitation)
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

            var invitedPlayer = _userService.GetUserByEmail(invitation.EmailTo);
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

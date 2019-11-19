using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    public class LeaderboardController : Controller
    {
        private IUserService _userService;
        public LeaderboardController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetTopUsers(10);
            return View(users);
        }
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicTacToe.Data;
using TicTacToe.Managers;
using TicTacToe.Models;

namespace TicTacToe.Services
{
    public class UserService : IUserService
    {
        private ILogger<UserService> _logger;
        private ApplicationUserManager _userManager;
        private DbContextOptions<GameDbContext> _dbContextOptions;
        private SignInManager<UserModel> _signInManager;
        //public UserService(DbContextOptions<GameDbContext> dbContextOptions)
        //{
        //    _dbContextOptions = dbContextOptions;
        //}
        public UserService(ApplicationUserManager userManager,   ILogger<UserService> logger , SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _logger = logger;

            var emailTokenProvider = new EmailTokenProvider<UserModel>();
            _userManager.RegisterTokenProvider("Default", emailTokenProvider);
            _signInManager = signInManager;
        }

        public async Task<SignInResult> SignInUser( LoginModel loginModel, HttpContext httpContext)
        {
            //var start = DateTime.Now;
            _logger.LogTrace($"signin user {loginModel.UserName}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var user =  await _userManager.FindByNameAsync(loginModel.UserName);
                var isValid = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, true);
                if (!isValid.Succeeded)
                {
                    return SignInResult.Failed;
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return SignInResult.NotAllowed;
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginModel.UserName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                identity.AddClaim(new Claim("displayName", $"{user.FirstName} {user.LastName}"));

                if (!string.IsNullOrEmpty(user.PhoneNumber))               
                    identity.AddClaim(new Claim(ClaimTypes.HomePhone, user.PhoneNumber));
                

                identity.AddClaim(new Claim("Score", user.Score.ToString()));

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                 new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = false });

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError($"can not sigin user{ loginModel.UserName} - { ex} "); 
              throw ex;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogTrace($"sigin user {loginModel.UserName} finished in { stopwatch.Elapsed} "); 
            }
        }

        public async Task SignOutUser(HttpContext httpContext)
        {
            await _signInManager.SignOutAsync();
            await httpContext.SignOutAsync(new AuthenticationProperties
            {
                IsPersistent = false
            });
            return;
        }

        public async Task<string> GetEmailConfirmationCode(UserModel user)
        {
            return
             await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<bool> ConfirmEmail(string email, string code)
        {
            var start = DateTime.Now;
            _logger.LogTrace($"Confirm email for user {email}");

            var stopwatch = new Stopwatch(); stopwatch.Start();

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return false;
                var result = await _userManager.ConfirmEmailAsync(user, code);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cannot confirm email for user {email} - {ex}");
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogTrace($"Confirm email for user finished in {stopwatch.Elapsed}");
            }
        }
        public async Task<bool> RegisterUser(UserModel userModel)
        {
            var start = DateTime.Now;
            _logger.LogTrace($"Start register user {userModel.Email} - {start}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                userModel.UserName = userModel.Email;
                var result = await _userManager.CreateAsync(userModel, userModel.Password);
                return result == IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cannot register user {userModel.Email} - {ex}");
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogTrace($"Start register user {userModel.Email} finished at {DateTime.Now} - elapsed {stopwatch.Elapsed.TotalSeconds} second(s)");
            }
        }
        public Task<bool> IsOnline(string name)
        {
            return Task.FromResult(true);
        }
       
        public async Task<UserModel> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsUserExisting(string email)
        {
            return (await _userManager.FindByEmailAsync(email)) != null;
        }

        public async Task<IEnumerable<UserModel>> GetTopUsers(int numberOfUsers)
        {
            return await _userManager.Users.OrderByDescending(x => x.Score).ToListAsync();
        }

        public async Task UpdateUser(UserModel userModel)
        {
            await _userManager.UpdateAsync(userModel);
        }
    }
}

using Application.Interfaces;
using Domain_Data.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProjectDetailsAPI_Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this._userManager = userManager;
            this._tokenRepository = tokenRepository;
        }
        //POST: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };
            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                //Add Roles to this user
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User Was registered! Please login");
                    }
                }
            }
            return BadRequest("Something went wrong! Please entered valid information");
        }

        //POST: /api/Auth/Login
        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);
            if (user != null)
            {
                var checkPassResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPassResult)
                {
                    //Get roles for the use
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles != null)
                    {
                        //Create token
                        var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                        var rsponse = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(jwtToken);
                    }

                    return Ok();
                }
            }
            return BadRequest("UserName or Password is incorrect");
        }

    }
}

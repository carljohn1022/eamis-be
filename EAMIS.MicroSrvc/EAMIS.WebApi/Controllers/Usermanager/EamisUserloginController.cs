using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response;
using EAMIS.Core.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EAMIS.Common.DTO.Masterfiles.EamisUserloginDTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]

    public class EamisUserloginController : ControllerBase
    {
        IRefreshTokenRepositories _refreshTokenRepositories;
        IEamisUserloginRepository _eamisUserloginRepository;
        RefreshTokenValidator _refreshTokenValidator;
        Authenticator _authenticator;

        public EamisUserloginController(IEamisUserloginRepository eamisUserloginRepository, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepositories refreshTokenRepositories, Authenticator authenticator)
        {
            _eamisUserloginRepository = eamisUserloginRepository;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepositories = refreshTokenRepositories;
            _authenticator = authenticator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginDTO>> Login([FromBody] UserLoginDTO item)
        {
            // if (await _eamisUserloginRepository.UserLoginExists(item.UsersToken.Username)) return BadRequest("This account is being connected already.");
            //if (!await _eamisUserloginRepository.UsernameExist(item.Username)) return BadRequest();
            //var LoginUser = await _eamisUserloginRepository.Login(item);
            //if (LoginUser == null) return Unauthorized();
            //if (LoginUser.UsersToken.isBlocked == true) return BadRequest("this is blocked user");
            //if(!await _eamisUserloginRepository.UsernameExist(item.Username))
            //{
            //    return NotFound("userName Not Found");
            //}
            if (await _eamisUserloginRepository.userBlocked(item.Username))
            {
                return Unauthorized("this userss is blocked");
            }
            var LoginUser = await _eamisUserloginRepository.Login(item);

            if (LoginUser == null)
            {
                return BadRequest("this user is null");
            }
            return Ok(LoginUser);
        }
        [HttpPut("DirectBlockedUser")]
        public async Task<ActionResult<UserLoginDTO>> DirectBlockedUser([FromBody]UserLoginDTO item)
        {
            var items = await _eamisUserloginRepository.DirectBlockedUser(item);
            if (items == null) return BadRequest();
            return Ok(item);
        }
        [HttpDelete("logout")]  
        public async Task<ActionResult<UserDTO>> Logout(int Id,[FromBody]UserDTO item)
        {
            var LoginUser = await _eamisUserloginRepository.Logout(Id,item);
            if (LoginUser == null) return Unauthorized();
            return Ok(LoginUser);
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginDTO>> Refresh([FromBody]LoginDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            bool isValidRefreshToken = _refreshTokenValidator.VerifyToken(item.UsersToken.RefreshToken);
            {
                if (!isValidRefreshToken)
                {
                    return BadRequest("Invalid Refresh Token");
                }
                RefreshTokenDTO refreshTokenDTO = await _refreshTokenRepositories.GetByToken(item.UsersToken.RefreshToken);
                if(refreshTokenDTO == null)
                {
                    return NotFound("Invalid refresh token");
                }
                LoginDTO user = await _eamisUserloginRepository.GetById(item);
                if(user == null)
                {
                    return NotFound("User not found token");
                }

                LoginDTO response = await _authenticator.User(user);
                return Ok(response);

            }
        }
        [HttpGet("GetIsLogout")]
        public async Task<string> GetIsLogout (int id)
        {
            var response = await _eamisUserloginRepository.GetSessionIsLogout(id);
            return response;
        }
    }
}

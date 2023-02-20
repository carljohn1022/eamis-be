using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using EAMIS.Core.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisUsersController : ControllerBase
    {

        IEamisUsersRepository _applicationUsers;
       
        public EamisUsersController(IEamisUsersRepository applicationUsers)
        {
            _applicationUsers = applicationUsers;
        }

        [HttpPost("register")]
        public async Task<ActionResult<EAMISUSERS>> Register(RegisterDTO item)
        {
            if (await _applicationUsers.Validate(item.AgencyEmployeeNumber))
                return Unauthorized();
            //if (await _applicationUsers.ValidateExistAgency(item.AgencyEmployeeNumber)) return BadRequest("Agency Number is already exist");
            if (await _applicationUsers.UserExists(item.Username))
                return Unauthorized();
            return Ok(await _applicationUsers.Register(item));

        }
        [HttpGet("publicsearch")]
        public async Task<ActionResult<EamisUsersDTO>> PublicSearch(string searchType, string searchKey, [FromQuery]PageConfig config)
        {
            return Ok(await _applicationUsers.PublicSearch(searchType,searchKey,config));
        }
       
        [HttpGet("list")]
        public async Task<ActionResult<EAMISUSERS>> List([FromQuery] EamisUsersDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisUsersDTO();
            return Ok(await _applicationUsers.List(filter, config));
        }
       [HttpGet("GetUsername")]
        public async Task<ActionResult<EAMISUSERS>> GetUsername([FromQuery]string Username)
        {
            return Ok(await _applicationUsers.GetUserName(Username));
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult<EamisUsersDTO>> ChangePassword([FromBody] EamisUsersDTO item, string NewPassword)
        {

            return Ok(await _applicationUsers.ChangePassword(item, NewPassword));
        }
        [HttpGet("AgencyName")]
        public async Task<string> GetAgencyName(string AgencyEmployeeNumber)
        {
            var response = await _applicationUsers.GetAgencyName(AgencyEmployeeNumber);
            return response;
        }
        [HttpGet("UserInfoId")]
        public async Task<string> UserInfoId(string AgencyEmployeeNumber)
        {
            var response = await _applicationUsers.GetId(AgencyEmployeeNumber);
            return response;
        }
        [HttpGet("getOfficeName")]
        public async Task<string> GetOfficeName(int officeId)
        {
            var response = await _applicationUsers.GetOfficeName(officeId);
            return response;
        }
    }
}

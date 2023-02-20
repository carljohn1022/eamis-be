using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Rolemanager
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisRoleModuleLinkController : ControllerBase
    {
        IEamisRoleModuleLinkRepository _eamisRoleModuleLinkRepository;
        public EamisRoleModuleLinkController(IEamisRoleModuleLinkRepository eamisRoleModuleLinkRepository)
        {
            _eamisRoleModuleLinkRepository = eamisRoleModuleLinkRepository;
        }
        [HttpGet("GetRoleId")]
        public async Task<ActionResult<EamisRoleModuleLinkDTO>> GetUserIdRole(int userId)
        {
            if (await _eamisRoleModuleLinkRepository.Validate(userId))
                return Unauthorized();
            return Ok(await _eamisRoleModuleLinkRepository.GetUserIdList(userId));

        }
        [HttpGet("AgencyName")]
        public async Task<string> GetAgencyName(int userId)
        {
            var response = await _eamisRoleModuleLinkRepository.GetAgencyName(userId);
            return response;
        }
        [HttpGet("GetUserId")]
        public async Task<ActionResult<EamisUserRolesDTO>> GetUserId(int userId)
        {
            if (_eamisRoleModuleLinkRepository.HasError)
                return BadRequest(_eamisRoleModuleLinkRepository.ErrorMessage);
            return Ok(await _eamisRoleModuleLinkRepository.GetUserIdList(userId));
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisRoleModuleLinkDTO>> List([FromQuery] EamisRoleModuleLinkDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisRoleModuleLinkDTO();
            return Ok(await _eamisRoleModuleLinkRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisRoleModuleLinkDTO>> Add([FromBody] EamisRoleModuleLinkDTO item)
        {
            if (item == null)
                item = new EamisRoleModuleLinkDTO();

            var result = await _eamisRoleModuleLinkRepository.Insert(item);
            if (_eamisRoleModuleLinkRepository.HasError)
                return BadRequest(_eamisRoleModuleLinkRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisRoleModuleLinkDTO>> Edit([FromBody] EamisRoleModuleLinkDTO item)
        {
            if (item == null)
                item = new EamisRoleModuleLinkDTO();
            var result = await _eamisRoleModuleLinkRepository.Update(item);
            if (_eamisRoleModuleLinkRepository.HasError)
                return BadRequest(_eamisRoleModuleLinkRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisRoleModuleLinkDTO>> Delete([FromBody] EamisRoleModuleLinkDTO item)
        {
            if (item == null)
                item = new EamisRoleModuleLinkDTO();
            var result = await _eamisRoleModuleLinkRepository.Delete(item);
            if (_eamisRoleModuleLinkRepository.HasError)
                return BadRequest(_eamisRoleModuleLinkRepository.ErrorMessage);
            return Ok(result);
        }
    }
}

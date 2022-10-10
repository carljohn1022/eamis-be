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

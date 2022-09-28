using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.ContractRepository.Rolemanager;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Rolemanager
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisModulesController : ControllerBase
    {
        IEamisModulesRepository _eamisModulesRepository;
        public EamisModulesController(IEamisModulesRepository eamisModulesRepository)
        {
            _eamisModulesRepository = eamisModulesRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisModulesDTO>> List([FromQuery] EamisModulesDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisModulesDTO();
            return Ok(await _eamisModulesRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisModulesDTO>> Add([FromBody] EamisModulesDTO item)
        {
            if (item == null)
                item = new EamisModulesDTO();

            var result = await _eamisModulesRepository.Insert(item);
            if (_eamisModulesRepository.HasError)
                return BadRequest(_eamisModulesRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisModulesDTO>> Edit([FromBody] EamisModulesDTO item)
        {
            if (item == null)
                item = new EamisModulesDTO();
            var result = await _eamisModulesRepository.Update(item);
            if (_eamisModulesRepository.HasError)
                return BadRequest(_eamisModulesRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisModulesDTO>> Delete([FromBody] EamisModulesDTO item)
        {
            if (item == null)
                item = new EamisModulesDTO();
            var result = await _eamisModulesRepository.Delete(item);
            if (_eamisModulesRepository.HasError)
                return BadRequest(_eamisModulesRepository.ErrorMessage);
            return Ok(result);
        }
    }
}

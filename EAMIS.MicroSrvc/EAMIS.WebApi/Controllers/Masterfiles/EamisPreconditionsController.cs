using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisPreconditionsController : ControllerBase
    {
        IEamisPrecondtionsRepository _eamisPrecondtionsRepository;
        public EamisPreconditionsController(IEamisPrecondtionsRepository eamisPrecondtionsRepository)
        {
            _eamisPrecondtionsRepository = eamisPrecondtionsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisPreconditionsDTO>> List([FromQuery] EamisPreconditionsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPreconditionsDTO();
            return Ok(await _eamisPrecondtionsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPreconditionsDTO>> Add([FromBody] EamisPreconditionsDTO item)
        {
            if (item == null)
                item = new EamisPreconditionsDTO();
            return Ok(await _eamisPrecondtionsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPreconditionsDTO>> Edit([FromBody] EamisPreconditionsDTO item)
        {
            if (item == null)
                item = new EamisPreconditionsDTO();
            return Ok(await _eamisPrecondtionsRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisPreconditionsDTO>> Delete([FromBody] EamisPreconditionsDTO item)
        {
            if (item == null)
                item = new EamisPreconditionsDTO();
            return Ok(await _eamisPrecondtionsRepository.Delete(item));
        }
    }
}

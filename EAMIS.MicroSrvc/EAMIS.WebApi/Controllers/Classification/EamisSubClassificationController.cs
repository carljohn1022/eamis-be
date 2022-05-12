using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Classification
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisSubClassificationController : ControllerBase
    {
        IEamisSubClassificationRepository _eamisSubClassificationRepository;
        public EamisSubClassificationController(IEamisSubClassificationRepository eamisSubClassificationRepository)
        {
            _eamisSubClassificationRepository = eamisSubClassificationRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisSubClassificationDTO>> List([FromQuery] EamisSubClassificationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisSubClassificationDTO();
            return Ok(await _eamisSubClassificationRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisSubClassificationDTO>> Add([FromBody] EamisSubClassificationDTO item)
        {
            if (item == null)
                item = new EamisSubClassificationDTO();
            return Ok(await _eamisSubClassificationRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisSubClassificationDTO>> Edit([FromBody] EamisSubClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisSubClassificationDTO();
            return Ok(await _eamisSubClassificationRepository.Update(item, Id));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisSubClassificationDTO>> Delete([FromBody] EamisSubClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisSubClassificationDTO();
            return Ok(await _eamisSubClassificationRepository.Delete(item, Id));
        }
    }
}

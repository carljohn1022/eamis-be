using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Classification
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisClassificationController : ControllerBase
    {
        IEamisClassificationRepository _eamisClassificationRepository;
        public EamisClassificationController(IEamisClassificationRepository eamisClassificationRepository)
        {
            _eamisClassificationRepository = eamisClassificationRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisClassificationDTO>> List([FromQuery] EamisClassificationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisClassificationDTO();
            return Ok(await _eamisClassificationRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisClassificationDTO>> Add([FromBody] EamisClassificationDTO item)
        {
            if (item == null)
                item = new EamisClassificationDTO();
            return Ok(await _eamisClassificationRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisClassificationDTO>> Edit([FromBody] EamisClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisClassificationDTO();
            return Ok(await _eamisClassificationRepository.Update(item, Id));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisClassificationDTO>> Delete([FromBody] EamisClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisClassificationDTO();
            return Ok(await _eamisClassificationRepository.Delete(item, Id));
        }
    }
}

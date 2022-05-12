using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Classification
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisGroupClassificationController : ControllerBase
    {
        IEamisGroupClassificationRepository _eamisGroupClassificationRepository;
        public EamisGroupClassificationController(IEamisGroupClassificationRepository eamisGroupClassificationRepository)
        {
            _eamisGroupClassificationRepository = eamisGroupClassificationRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisGroupClassificationDTO>> List([FromQuery] EamisGroupClassificationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisGroupClassificationDTO();
            return Ok(await _eamisGroupClassificationRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisGroupClassificationDTO>> Add([FromBody] EamisGroupClassificationDTO item)
        {
            if (item == null)
                item = new EamisGroupClassificationDTO();
            return Ok(await _eamisGroupClassificationRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisGroupClassificationDTO>> Edit([FromBody] EamisGroupClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisGroupClassificationDTO();
            return Ok(await _eamisGroupClassificationRepository.Update(item, Id));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisGroupClassificationDTO>> Delete([FromBody] EamisGroupClassificationDTO item, int Id)
        {
            if (item == null)
                item = new EamisGroupClassificationDTO();
            return Ok(await _eamisGroupClassificationRepository.Delete(item, Id));
        }
    }
}

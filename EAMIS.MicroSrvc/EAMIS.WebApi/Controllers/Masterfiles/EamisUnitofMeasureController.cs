using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisUnitofMeasureController : ControllerBase
    {
        IEamisUnitofMeasureRepository _eamisUnitofMeasureRepository;
        public EamisUnitofMeasureController(IEamisUnitofMeasureRepository eamisUnitofMeasureRepository)
        {
            _eamisUnitofMeasureRepository = eamisUnitofMeasureRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisUnitofMeasureDTO>> List([FromQuery] EamisUnitofMeasureDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisUnitofMeasureDTO();
            return Ok(await _eamisUnitofMeasureRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisUnitofMeasureDTO>> Add([FromBody] EamisUnitofMeasureDTO item)
        {
            if (await _eamisUnitofMeasureRepository.ValidateExistDesc(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            else if (await _eamisUnitofMeasureRepository.ValidationForUomExistShortDescNotExist(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            else if (await _eamisUnitofMeasureRepository.ValidationForShortDescExistUomNotExist(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisUnitofMeasureDTO();
            return Ok(await _eamisUnitofMeasureRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisUnitofMeasureDTO>> Edit([FromBody] EamisUnitofMeasureDTO item)
        {
            var data = new EamisUnitofMeasureDTO();
            if (await _eamisUnitofMeasureRepository.ValidateExistDesc(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            else if (await _eamisUnitofMeasureRepository.ValidationForUomExistShortDescNotExist(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            else if (await _eamisUnitofMeasureRepository.ValidationForShortDescExistUomNotExist(item.Short_Description, item.Uom_Description))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisUnitofMeasureDTO();
            return Ok(await _eamisUnitofMeasureRepository.Update(item));
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISUNITOFMEASURE>> Search(string type, string searchValue)
        {
            return Ok(await _eamisUnitofMeasureRepository.SearchMeasure(type, searchValue));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisUnitofMeasureDTO>> Delete([FromBody] EamisUnitofMeasureDTO item)
        {
            if (item == null)
                item = new EamisUnitofMeasureDTO();
            return Ok(await _eamisUnitofMeasureRepository.Delete(item));
        }
    }
}

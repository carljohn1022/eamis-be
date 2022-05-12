using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisWarehouseController : ControllerBase
    {
        IEamisWarehouseRepository _eamisWarehouseRepository;
        public EamisWarehouseController(IEamisWarehouseRepository eamisWarehouseRepository)
        {
            _eamisWarehouseRepository = eamisWarehouseRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisWarehouseDTO>> List([FromQuery] EamisWarehouseDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisWarehouseDTO();
            return Ok(await _eamisWarehouseRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisWarehouseDTO>> Add([FromBody] EamisWarehouseDTO item)
        {
            if (await _eamisWarehouseRepository.ValidateExistingCode(item.Warehouse_Description))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisWarehouseDTO();
            return Ok(await _eamisWarehouseRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisWarehouseDTO>> Edit([FromBody] EamisWarehouseDTO item)
        {
            if (item == null)
                item = new EamisWarehouseDTO();
            return Ok(await _eamisWarehouseRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisWarehouseDTO>> Delete([FromBody] EamisWarehouseDTO item)
        {
            if (item == null)
                item = new EamisWarehouseDTO();
            return Ok(await _eamisWarehouseRepository.Delete(item));
        }
    }
}

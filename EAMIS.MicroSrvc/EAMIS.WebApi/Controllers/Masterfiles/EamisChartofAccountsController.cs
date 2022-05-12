using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController] 
    public class EamisChartofAccountsController : ControllerBase
    {
        IEamisChartofAccountsRepository _eamisChartofAccountsRepository;
        public EamisChartofAccountsController(IEamisChartofAccountsRepository eamisChartofAccountsRepository)
        {
            _eamisChartofAccountsRepository = eamisChartofAccountsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisChartofAccountsDTO>> List([FromQuery] EamisChartofAccountsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisChartofAccountsDTO();
            return Ok(await _eamisChartofAccountsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisChartofAccountsDTO>> Add([FromBody] EamisChartofAccountsDTO item)
        {
            if (await _eamisChartofAccountsRepository.ValidateExistingCode(item.AccountCode))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisChartofAccountsDTO();
            return Ok(await _eamisChartofAccountsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisChartofAccountsDTO>> Edit([FromBody] EamisChartofAccountsDTO item, int Id)
        {
            if (item == null)
                item = new EamisChartofAccountsDTO();
            return Ok(await _eamisChartofAccountsRepository.Update(item,Id));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisChartofAccountsDTO>> Delete([FromBody] EamisChartofAccountsDTO item, int Id)
        {
            if (item == null)
                item = new EamisChartofAccountsDTO();
            return Ok(await _eamisChartofAccountsRepository.Delete(item,Id));
        }
    }
}

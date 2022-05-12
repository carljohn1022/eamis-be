using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisGeneralChartofAccountsController : ControllerBase
    {
        IEamisGeneralChartofAccountsRepository _eamisGeneralChartofAccountsRepository;
        public EamisGeneralChartofAccountsController(IEamisGeneralChartofAccountsRepository eamisGeneralChartofAccountsRepository)
        {
            _eamisGeneralChartofAccountsRepository = eamisGeneralChartofAccountsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisGeneralChartofAccountsDTO>> List([FromQuery] EamisGeneralChartofAccountsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisGeneralChartofAccountsDTO();
            return Ok(await _eamisGeneralChartofAccountsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisGeneralChartofAccountsDTO>> Add([FromBody] EamisGeneralChartofAccountsDTO item)
        {
            if (item == null)
                item = new EamisGeneralChartofAccountsDTO();
            return Ok(await _eamisGeneralChartofAccountsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisGeneralChartofAccountsDTO>> Edit([FromBody] EamisGeneralChartofAccountsDTO item, int Id)
        {
            if (item == null)
                item = new EamisGeneralChartofAccountsDTO();
            return Ok(await _eamisGeneralChartofAccountsRepository.Update(item,Id));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisGeneralChartofAccountsDTO>> Delete([FromBody] EamisGeneralChartofAccountsDTO item,int Id)
        {
            if (item == null)
                item = new EamisGeneralChartofAccountsDTO();
            return Ok(await _eamisGeneralChartofAccountsRepository.Delete(item, Id));
        }
    }
}


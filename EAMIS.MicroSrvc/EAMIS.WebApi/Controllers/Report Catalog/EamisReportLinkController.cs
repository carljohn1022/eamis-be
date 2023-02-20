using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report;
using EAMIS.Common.DTO.Report_Catalog;
//using EAMIS.Core.ContractRepository.Report.Catalog;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisReportLinkController : ControllerBase
    {
        private readonly IEamisReportLinkRepository _eamisReportLinkRepository;
        public EamisReportLinkController(IEamisReportLinkRepository eamisReportLinkRepository)
        {
            _eamisReportLinkRepository = eamisReportLinkRepository;
        }


        [HttpGet("list")]
        public async Task<ActionResult<EAMISUSERREPORTLINK>> List([FromQuery] EamisUserReportLinkDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisUserReportLinkDTO();
            return Ok(await _eamisReportLinkRepository.List(filter, config));
        }
        [HttpGet("GetUserId")]
        public async Task<ActionResult<EamisUsersDTO>> GetUserId(int userId)
        {
            return Ok(await _eamisReportLinkRepository.GetUserIdList(userId));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisUserReportLinkDTO>> Add([FromBody] EamisUserReportLinkDTO item)
        {
            if (item == null)
                item = new EamisUserReportLinkDTO();
            var result = await _eamisReportLinkRepository.Insert(item);
            if (_eamisReportLinkRepository.HasError)
                return BadRequest(_eamisReportLinkRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisUserReportLinkDTO>> Edit([FromBody] EamisUserReportLinkDTO item)
        {
            if (item == null)
                item = new EamisUserReportLinkDTO();
            var result = await _eamisReportLinkRepository.Update(item);
            if (_eamisReportLinkRepository.HasError)
                return BadRequest(_eamisReportLinkRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisUserReportLinkDTO>> Delete([FromBody] EamisUserReportLinkDTO item)
        {
            if (item == null)
                item = new EamisUserReportLinkDTO();
            var result = await _eamisReportLinkRepository.Delete(item);
            if (_eamisReportLinkRepository.HasError)
                return BadRequest(_eamisReportLinkRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
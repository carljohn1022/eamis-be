using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisProcurementCategoryController : ControllerBase
    {
        IEamisProcurementCategoryRepository _eamisProcurementCategoryRepository;
        public EamisProcurementCategoryController(IEamisProcurementCategoryRepository eamisProcurementCategoryRepository)
        {
            _eamisProcurementCategoryRepository = eamisProcurementCategoryRepository;
        }

        [HttpGet("SearchProcurementCategory")]
        public async Task<ActionResult<EAMISPROCUREMENTCATEGORY>> SearchProcurementCategory(string searchType, string searchValue)
        {
            return Ok(await _eamisProcurementCategoryRepository.SearchProcurementCategory(searchType, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROCUREMENTCATEGORY>> List([FromQuery] EamisProcurementCategoryDTO filter, [FromQuery]PageConfig config)
        {
            if (filter == null)
                filter = new EamisProcurementCategoryDTO();
            return Ok(await _eamisProcurementCategoryRepository.List(filter,config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisProcurementCategoryDTO>> Add([FromBody] EamisProcurementCategoryDTO item)
        {
            if (item == null)
                item = new EamisProcurementCategoryDTO();
            return Ok(await _eamisProcurementCategoryRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisProcurementCategoryDTO>> Edit([FromQuery] int Id, [FromBody] EamisProcurementCategoryDTO item)
        {
            if (item == null)
                item = new EamisProcurementCategoryDTO();
            return Ok(await _eamisProcurementCategoryRepository.Update(Id, item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisProcurementCategoryDTO>> Delete([FromBody] EamisProcurementCategoryDTO item)
        {
            if (item == null)
                item = new EamisProcurementCategoryDTO();
            return Ok(await _eamisProcurementCategoryRepository.Delete(item));
        }
    }
}

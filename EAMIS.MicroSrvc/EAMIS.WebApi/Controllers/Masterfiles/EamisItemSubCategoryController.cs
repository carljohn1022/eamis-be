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
    public class EamisItemSubCategoryController : ControllerBase
    {
        IEamisItemSubCategoryRepository _eamisItemSubCategoryRepository;
        public EamisItemSubCategoryController(IEamisItemSubCategoryRepository eamisItemSubCategoryRepository)
        {
            _eamisItemSubCategoryRepository = eamisItemSubCategoryRepository;
        }

        [HttpGet("SearchItemSubCategory")]
        public async Task<ActionResult<EAMISITEMSUBCATEGORY>> SearchItemSubCategory(string type, string searchValue)
        {
            return Ok(await _eamisItemSubCategoryRepository.SearchItemSubCategory(type, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISITEMSUBCATEGORY>> List([FromQuery] EamisItemSubCategoryDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.List(filter, config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Add([FromBody] EamisItemSubCategoryDTO item)
        {
            if (await _eamisItemSubCategoryRepository.Validation(item.SubCategoryName))
            {
                //
                return Unauthorized();
            }
            if (item == null)
                item = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.Insert(item));


        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Edit([FromBody] EamisItemSubCategoryDTO item, int id)
        {
            var data = new EamisItemSubCategoryDTO();
            if (await _eamisItemSubCategoryRepository.ValidateExistingSubUpdate(item.SubCategoryName, item.Id))
            {
                if (item == null)
                    item = new EamisItemSubCategoryDTO();
                return Ok(await _eamisItemSubCategoryRepository.Update(item, id));
            }
            else if (await _eamisItemSubCategoryRepository.Validation(item.SubCategoryName))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisItemSubCategoryRepository.Update(item, id));
            }

        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Delete([FromBody] EamisItemSubCategoryDTO item)
        {
            if (item == null)
                item = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.Delete(item));
        }
    }
}

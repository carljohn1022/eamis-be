using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Authorization;
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
    public class EamisPropertyItemsController : ControllerBase
    {
        IEamisPropertyItemsRepository _eamisPropertyItemsRepository;
        IEamisItemCategoryRepository _eamisItemCategoryRepository;
        public EamisPropertyItemsController(IEamisPropertyItemsRepository eamisPropertyItemsRepository, IEamisItemCategoryRepository eamisItemCategoryRepository)
        {
            _eamisPropertyItemsRepository = eamisPropertyItemsRepository;
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
        }

        [HttpGet("GeneratedProperty")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> GeneratedProperty()
        {
            return Ok(await _eamisPropertyItemsRepository.GeneratedProperty());
        }

        [HttpGet("PublicSearchPropertyItems")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> PublicSearchPropertyItems(string type,string SearchValue)
        {
            return Ok(await _eamisPropertyItemsRepository.PublicSearch(type,SearchValue));

        }
       
        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> List([FromQuery] EamisPropertyItemsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Add([FromBody] EamisPropertyItemsDTO item)
        {
            if(await _eamisPropertyItemsRepository.ValidateExistingItem(item.PropertyNo))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Edit([FromBody] EamisPropertyItemsDTO item)
        {
            var data = new EamisPropertyItemsDTO();
            if (await _eamisPropertyItemsRepository.UpdateValidateExistingItem(item.PropertyNo, item.Id))
            {
                if (item == null)
                    item = new EamisPropertyItemsDTO();
                return Ok(await _eamisPropertyItemsRepository.Update(item));
            }
            else if (await _eamisPropertyItemsRepository.ValidateExistingItem(item.PropertyNo))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisPropertyItemsRepository.Update(item));
            }
        }
        [HttpGet("getPropertyNo")]
        public async Task<string> GetSupplier(int categoryId)
        {
            var response = await _eamisItemCategoryRepository.GetPropertyNo(categoryId);
            return response;
        }
    }

        
   }


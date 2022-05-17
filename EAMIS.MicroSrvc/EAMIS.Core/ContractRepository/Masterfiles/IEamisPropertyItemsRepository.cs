using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisPropertyItemsRepository
    {
        Task<DataList<EamisPropertyItemsDTO>> PublicSearch(string type,string SearchValue);
        Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config);
        Task<EamisPropertyItemsDTO> Insert(EamisPropertyItemsDTO item);
        Task<EamisPropertyItemsDTO> Update(EamisPropertyItemsDTO item, int id);
        Task<EamisPropertyItemsDTO> Delete(EamisPropertyItemsDTO item);
        Task<EamisPropertyItemsDTO> GeneratedProperty();
        Task<bool> ValidateExistingItem(string propertyNo);
        Task<bool> UpdateValidateExistingItem(string propertyNo, int id);
    }
}

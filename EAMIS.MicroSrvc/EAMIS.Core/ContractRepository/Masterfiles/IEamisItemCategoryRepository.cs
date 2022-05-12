using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisItemCategoryRepository
    {
        //Task<DataList<EamisItemCategoryDTO>> MapToDTOList(string searchKey, PageConfig config);
        Task<DataList<EamisItemCategoryDTO>> SearchItemCategory(string searchValue);
        Task<DataList<EamisItemCategoryDTO>> List(EamisItemCategoryDTO filter, PageConfig config);
        Task<EamisItemCategoryDTO> Insert(EamisItemCategoryDTO item);
        Task<EamisItemCategoryDTO> Update(EamisItemCategoryDTO item);
        Task<EamisItemCategoryDTO> Delete(EamisItemCategoryDTO item);
    }
}

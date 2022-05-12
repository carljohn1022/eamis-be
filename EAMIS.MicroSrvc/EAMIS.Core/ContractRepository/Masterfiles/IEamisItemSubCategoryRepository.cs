using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisItemSubCategoryRepository
    {
        Task<DataList<EamisItemSubCategoryDTO>> SearchItemSubCategory(string type, string searchValue);
        Task<DataList<EamisItemSubCategoryDTO>> List(EamisItemSubCategoryDTO filter, PageConfig config);
        Task<EamisItemSubCategoryDTO> Insert(EamisItemSubCategoryDTO item);
        Task<EamisItemSubCategoryDTO> Update(EamisItemSubCategoryDTO item);
        Task<EamisItemSubCategoryDTO> Delete(EamisItemSubCategoryDTO item);
        Task<bool> ValidateExistingSub(int categoryId);
        Task<bool> ValidateExistingCategoryId(int categoryId);
        Task<bool> ValidateExistingSubUpdate(string SubCategoryName, int CategoryId);
        Task<bool> Validation(int categoryId, string subCategoryName);
    }
}

using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUnitofMeasureRepository
    {
        Task<DataList<EamisUnitofMeasureDTO>> List(EamisUnitofMeasureDTO filter, PageConfig config);
        Task<EamisUnitofMeasureDTO> Insert(EamisUnitofMeasureDTO item);
        Task<EamisUnitofMeasureDTO> Update(EamisUnitofMeasureDTO item, int id);
        Task<EamisUnitofMeasureDTO> Delete(EamisUnitofMeasureDTO item);
        Task<bool> ValidateExistingDesc(string Short_Description, string Uom_Description);
        Task<bool> UpdateValidateExistingDesc(string Short_Description, string Uom_Description, int id);
        Task<DataList<EamisUnitofMeasureDTO>> SearchMeasure(string type, string searchValue);
    }
}

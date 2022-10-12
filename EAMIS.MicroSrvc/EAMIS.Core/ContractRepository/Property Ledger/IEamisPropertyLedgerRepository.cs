using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyLedgerRepository
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }

        Task<EamisPropertyLedgerDTO> Delete(EamisPropertyLedgerDTO item);
        Task<EamisPropertyLedgerDTO> Insert(EamisPropertyLedgerDTO item);
        Task<DataList<EamisPropertyLedgerDTO>> List(EamisPropertyLedgerDTO filter, PageConfig config);
        Task<DataList<EamisPropertyLedgerDTO>> ListForCreation(EamisPropertyLedgerDTO filter, PageConfig config);
        Task<EamisPropertyLedgerDTO> Update(EamisPropertyLedgerDTO item);
    }
}
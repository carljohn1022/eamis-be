using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyDetailsRepository
    {
        Task<DataList<EamisPropertyDetailsDTO>> List(EamisPropertyDetailsDTO filter,PageConfig config);
        Task<EamisPropertyDetailsDTO> Insert(EamisPropertyDetailsDTO item);
        Task<EamisPropertyDetailsDTO> Update(EamisPropertyDetailsDTO item);
        Task<EamisPropertyDetailsDTO> Delete(EamisPropertyDetailsDTO item);
    }
}

using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisNoticeofDeliveryRepository
    {
        Task<DataList<EamisNoticeofDeliveryDTO>> List(EamisNoticeofDeliveryDTO filter,PageConfig config);
        Task<EamisNoticeofDeliveryDTO> Insert(EamisNoticeofDeliveryDTO item);
        Task<EamisNoticeofDeliveryDTO> Update(EamisNoticeofDeliveryDTO item);
        Task<EamisNoticeofDeliveryDTO> Delete(EamisNoticeofDeliveryDTO item);
    }
}

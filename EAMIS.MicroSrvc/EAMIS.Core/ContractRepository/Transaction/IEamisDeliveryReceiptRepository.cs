using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
   public interface IEamisDeliveryReceiptRepository
    {
        Task<DataList<EamisDeliveryReceiptDTO>> List(EamisDeliveryReceiptDTO filter, PageConfig config);
        Task<EamisDeliveryReceiptDTO> Insert(EamisDeliveryReceiptDTO item);
        Task<EamisDeliveryReceiptDTO> Update(EamisDeliveryReceiptDTO item);
        Task<EamisDeliveryReceiptDTO> Delete(EamisDeliveryReceiptDTO item);
    }
}

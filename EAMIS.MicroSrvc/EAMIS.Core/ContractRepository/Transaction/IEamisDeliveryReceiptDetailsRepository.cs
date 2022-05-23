﻿using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisDeliveryReceiptDetailsRepository
    {
        Task<DataList<EamisDeliveryReceiptDetailsDTO>> List(EamisDeliveryReceiptDetailsDTO filter, PageConfig config);
        Task<EamisDeliveryReceiptDetailsDTO> Insert(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisDeliveryReceiptDetailsDTO> Update(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisDeliveryReceiptDetailsDTO> Delete(EamisDeliveryReceiptDetailsDTO item);
    }
}
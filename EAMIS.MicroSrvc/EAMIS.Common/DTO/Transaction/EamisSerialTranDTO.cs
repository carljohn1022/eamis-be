using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
   public class EamisSerialTranDTO
    {
        public int Id { get; set; }
        public int DeliveryReceiptDetailsId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime WarrantyExpiryDate { get; set; }
    }
}

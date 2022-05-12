using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisNoticeofDeliveryDTO
    {
        public int Id { get; set; }
        public int TransactioId { get; set; }
        public int PurchaseRequestNo { get; set; }
        public int PurchaseOrderNo { get; set; }
        public int PropertyDetails_Id { get; set; }
        public int DeliveryDate { get; set; }
        public string InspectionType { get; set; }
        public bool IsWaterMaterial { get; set; }
        public bool IsWaranttyCertificate { get; set; }
        public bool IsWrongProperty { get; set; }
        public bool IsInCompleteProperty { get; set; }
        public int UserId { get; set; }
    }
}

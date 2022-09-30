using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisServiceLogDetailsDTO
    {
        public int ID { get; set; }
        public int ServiceLogId { get; set; }
        public int ReceivingTransactionId { get; set; }
        public string PropertyNumber { get; set; }
        public string PropertyDescription { get; set; }
        public string AssetCondition { get; set; }
        public decimal ReceivingAmount { get; set; }
        public int SupplierId { get; set; }
        public string SupplierDescription { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AssessedValue { get; set; }
        public decimal AppraisedValue { get; set; }
        public decimal AppraisalIncrement { get; set; }
        public decimal RealEstateTaxPayment { get; set; }
        public decimal AreaSQM { get; set; }
        public string Notes { get; set; }
        public string TranType { get; set; }
        public EamisSupplierDTO SupplierGroup { get; set; } = new EamisSupplierDTO();
        public EamisPropertyTransactionDTO ReceivingGroup { get; set; } = new EamisPropertyTransactionDTO();
        public EamisServiceLogDTO ServiceLogGroup { get; set; } = new EamisServiceLogDTO();
    }
}

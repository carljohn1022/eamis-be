using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyLedgerDTO
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string UOM { get; set; }
        public string ResponsibilityCode { get; set; }
        public decimal UnitCost { get; set; }
        public int TotalReceived { get; set; }
        public int TotalIssued { get; set; }
        public int TotalOnHandDR { get; set; }
        public decimal TotalValueOH { get; set; }
        public int PhysicalCounnt { get; set; }
        public decimal TotalValuePC { get; set; }
        public int Variance { get; set; }
        public decimal TotalValueVar { get; set; }
        public string Remarks { get; set; }
        public DateTime AsOfDate { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyScheduleDTO
    {
        public int Id { get; set; }
        public string PropertyNumber { get; set; } //to verify the correct data type with Justin
        public string Status { get; set; }
        public string ItemDescription { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public bool ForDepreciation { get; set; }
        public decimal AreaSQM { get; set; }
        public string SerialNo { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string LastDepartment { get; set; }
        public int Warranty { get; set; }
        public string InvoiceNo { get; set; } //to verify the correct data type with Justin
        public int SvcAgreementNo { get; set; }
        public string VendorName { get; set; }
        public string AssetTag { get; set; }
        public string CostCenter { get; set; }
        public string Names { get; set; }
        public string Details { get; set; }
        public DateTime WarrantyDate { get; set; }
        public DateTime RRDate { get; set; }
        public string AssetCondition { get; set; }
        public decimal RevaluationCost { get; set; }
        public decimal DisposedAmount { get; set; }
        public DateTime AcquisitionDate { get; set; }

        public string PORef { get; set; }
        public string RRRef { get; set; }
        public decimal AcquisitionCost { get; set; }
        public decimal SalvageValue { get; set; }
        public int ESTLife { get; set; }
        public decimal DeprecAmount { get; set; }
        public decimal DepreciableCost { get; set; }        //public decimal BookValue { get; set; }
        public decimal BookValue { get; set; }
        public decimal AssessedValue { get; set; }
        public decimal AppraisedValue { get; set; }
        public decimal AppraisalIncrement { get; set; }
        public decimal RealEstateTaxPayment { get; set; }
        public DateTime LastPostedDate { get; set; }
        public string ItemCode { get; set; }
        public int RemainingLife { get; set; }
        public int ReferenceId { get; set; } //Property Transaction Details ID
        public decimal AccumulatedDepreciationAmount { get; set; }
    }
}

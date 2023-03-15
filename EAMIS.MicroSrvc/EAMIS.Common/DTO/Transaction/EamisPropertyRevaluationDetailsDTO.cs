using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyRevaluationDetailsDTO
    {
        public int Id { get; set; }
        public int PropertyRevaluationId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal AcquisitionCost { get; set; }
        public DateTime Depreciation { get; set; }
        public int RemainingLife { get; set; }
        public decimal  AccumulativeDepreciation { get; set; }
        public string PrevRevaluation  { get; set; }
        public decimal NetBookValue  { get; set; }
        public decimal RevaluedAmount  { get; set; }
        public decimal FairValue  { get; set; }
        public DateTime NewDep  { get; set; }
        public decimal SalvageValue { get; set; }
        public decimal DepPerMonth { get; set; }
        public decimal NewDepPerMonth { get; set; }
        public string UserStamp { get; set; }
        public EamisPropertyRevaluationDTO PropertyRevaluation { get; set; }
    }
}

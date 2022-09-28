using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYREVALUATIONDETAILS
    {
        public int ID { get; set; }
        public int PROPERTY_REVALUATION_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public decimal ACQ_COST { get; set; }
        public decimal SALVAGE_VALUE { get; set; }
        public DateTime DEPRECIATION { get; set; }
        public int REMAINING_LIFE { get; set; }
        public decimal ACCUMULATIVE_DEPRECIATION { get; set; }
        public string PREV_REVALUATION { get; set; }
        public decimal NET_BOOK_VALUE { get; set; }
        public decimal REVALUED_AMT { get; set; }
        public decimal FAIR_VALUE { get; set; }
        public DateTime NEW_DEP { get; set; }
        public decimal NEW_DEP_PER_MONTH { get; set; }
        public decimal DEP_PER_MONTH { get; set; }

    }
}

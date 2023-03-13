using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISCHARTOFACCOUNTS
    {
        public int ID { get; set; }
        public int GROUP_ID { get; set; }
        public string OBJECT_CODE { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public bool IS_PART_OF_INVENTORY { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string GENERAL_LEDGER_ACCOUNT { get; set; }
        public string USER_STAMP { get; set; }
        public EAMISGROUPCLASSIFICATION GROUPCLASSIFICATION { get; set; }
        public EAMISSUBCLASSIFICATION SUBCLASSIFICATION { get; set; }
        public EAMISCLASSIFICATION CLASSIFICATION { get; set; }
        public List<EAMISITEMCATEGORY> ITEM_CATEGORY { get; set; }

    }
}
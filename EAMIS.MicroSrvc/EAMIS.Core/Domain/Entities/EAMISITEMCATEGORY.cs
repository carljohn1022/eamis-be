using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISITEMCATEGORY
    {
        [Key]
        public int ID { get; set; }
        public string SHORT_DESCRIPTION { get; set; }
        public int CHART_OF_ACCOUNT_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public bool IS_STOCKABLE { get; set; }
        public int STOCK_QUANTITY { get; set; }
        public int ESTIMATED_LIFE { get; set; }
        public string COST_METHOD { get; set; } //Formula...... another discussion.
        public string DEPRECIATION_METHOD { get; set; } //Formula... another discussion.
        public bool IS_SERIALIZED { get; set; }
        public bool IS_ASSET { get; set; }
        public bool IS_SUPPLIES { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool FOR_DEPRECIATION { get; set; }
        public string USER_STAMP { get; set; }
        public EAMISCHARTOFACCOUNTS CHART_OF_ACCOUNTS { get; set; }
        public List<EAMISITEMSUBCATEGORY> ITEM_SUB_CATEGORY { get; set; }
        public List<EAMISPROPERTYITEMS> PROPERTY_ITEMS { get; set; }


    }
}

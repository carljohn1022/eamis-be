using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYITEMS
    {
        [Key]
        public int ID { get; set; }
        public string APP_NO { get; set; }
        public string PROPERTY_NO { get; set; }
        public string PROPERTY_NAME { get; set; }
        public int CATEGORY_ID { get; set; }
        public int SUBCATEGORY_ID { get; set; }
        public string BRAND { get; set; }
        public int UOM_ID { get; set; }
        public int WAREHOUSE_ID { get; set; }
        public string PROPERTY_TYPE { get; set; }
        public string MODEL { get; set; }
        public int QUANTITY { get; set; }
        public int SUPPLIER_ID { get; set; }
        public bool IS_ACTIVE { get; set; }
        public EAMISITEMCATEGORY ITEM_CATEGORY { get; set; }
        public EAMISUNITOFMEASURE UOM_GROUP { get; set; }
        public EAMISWAREHOUSE WAREHOUSE_GROUP { get; set; }
        public EAMISSUPPLIER SUPPLIER_GROUP { get; set; }

        //public int SUB_CATEGORY_ID { get; set; }
        //public string PROPERTY_NAME { get; set; }
        //public string BRAND { get; set; }
        //public string MODEL { get; set; }
        //public string SERIAL_NUMBER { get; set; }
        //public decimal UNIT_COST { get; set; }
        //public int UOM_ID { get; set; }
        //public int QUANTITY_IN_STOCK { get; set; }
        //public int PROPERTY_TYPE_ID { get; set; }
        //public EAMISITEMSUBCATEGORY SUB_CATEGORY_GROUP { get; set; }
        //public EAMISUNITOFMEASURE UOM_GROUP { get; set; }



        //public EAMISPROPERTYITEMS PARENT { get; set; }
        //public ICollection<EAMISPROPERTYITEMS> SUB_PROPERTY_ITEM { get; } = new List<EAMISPROPERTYITEMS>();

    }
}
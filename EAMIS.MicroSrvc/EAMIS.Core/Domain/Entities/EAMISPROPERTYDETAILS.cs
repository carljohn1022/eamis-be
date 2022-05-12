using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYDETAILS
    {
        public int ID { get; set; }
        public int PROPERTY_TYPE_ID { get; set; }
        public string PROPERTY_NAME { get; set; }
        public string BRAND { get; set; }
        public string MODEL_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public decimal UNIT_COST { get; set; }
        public int UOM_ID { get; set; }
        public double STOCK_NO { get; set; }
        public bool IS_STOCKABLE { get; set; }
        public double QTY_IN_STOCK { get; set; }
        public int SOURCE_ID { get; set; }
        public bool IS_PROPERTY { get; set; }

    }
}

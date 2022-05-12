using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISNOTICEOFDELIVERY
    {
        public int ID { get; set; }
        public int TRANSACTION_ID { get; set; }
        public int PURCHASE_REQUEST_NO { get; set; }
        public int PUCHASE_ORDER_NO { get; set; }
        public int PROPERTY_DETAILS_ID { get; set; }
        public int DELIVERY_DATE { get; set; }
        public string INSPECTION_TYPE { get; set; }
        public bool IS_WATER_MATERAIL { get; set; }
        public bool IS_WARRANTY_CERTIFICATE { get; set; }
        public bool IS_WRONG_PROPERTY { get; set; }
        public bool IS_INCOMPLETE_PROPERTY { get; set; }
        public int USER_ID { get; set; }


    }
}

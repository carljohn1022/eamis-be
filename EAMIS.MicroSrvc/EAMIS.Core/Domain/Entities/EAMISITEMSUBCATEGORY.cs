using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISITEMSUBCATEGORY
    {
        [Key]
        public int ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public EAMISITEMCATEGORY ITEM_CATEGORY { get; set; }
        public EAMISPROPERTYITEMS PROPERTY_ITEM { get; set; }
        
    }
}

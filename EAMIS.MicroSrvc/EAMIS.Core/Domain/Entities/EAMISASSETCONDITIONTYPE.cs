using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
   public class EAMISASSETCONDITIONTYPE
    {
        [Key]
        public int ID { get; set; }
        public string ASSET_CONDITION_DESC { get; set; }
    }
}

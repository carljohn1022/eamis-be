using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
   public class EAMISRESPONSIBILITYCODE
    {
        [Key]
        public int ID { get; set; }
        public string OFFICE { get; set; }
        public string DEPARTMENT { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
}

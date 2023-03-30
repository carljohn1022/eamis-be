using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISBRANCHMAINTENANCE
    {
        [Key]
        public int SeqID { get; set; }
        public string BranchID { get; set; }
        public string BranchDescription { get; set; }
        public string Region { get; set; }
        public string AreaID { get; set; }
        public string AreaDescription { get; set; }
    }
}

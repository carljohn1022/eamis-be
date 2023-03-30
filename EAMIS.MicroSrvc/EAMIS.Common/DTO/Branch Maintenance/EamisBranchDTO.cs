using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Branch_Maintenance
{
    public class EamisBranchDTO
    {
        [Key]
        public int SeqID { get; set; }
        public string BranchID { get; set; }
        public string BranchDescription { get; set; }
        public string Region { get; set; }
        public string AreaID { get; set; }
        public string AreaDescription { get; set; }
        public EamisRegionDTO RegionGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities.AIS
{
    public class AISPOSITIONVIEW
    {
        [Key]
        public string? AgencyEmployeeNumber { get; set; }
        public int? PositionId { get; set; }
        public string? Position { get; set; }
    }
}

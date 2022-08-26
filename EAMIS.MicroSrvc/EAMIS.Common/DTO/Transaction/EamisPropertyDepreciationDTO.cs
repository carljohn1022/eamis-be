using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyDepreciationDTO
    {
        public int Id { get; set; }
        public int PropertyScheduleId { get; set; }
        public decimal DepreciationAmount { get; set; }
        public DateTime PostingDate { get; set; }
        public int DepreciationYear { get; set; }
        public int DepreciationMonth { get; set; }

        public EamisPropertyScheduleDTO PropertyScheduleDetails { get; set; }
    }
}

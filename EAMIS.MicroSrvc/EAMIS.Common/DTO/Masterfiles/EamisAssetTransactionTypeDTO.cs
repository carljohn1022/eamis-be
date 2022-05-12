using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAssetTransactionTypeDTO
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public int StartSeries { get; set; }
        public int EndSeries { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

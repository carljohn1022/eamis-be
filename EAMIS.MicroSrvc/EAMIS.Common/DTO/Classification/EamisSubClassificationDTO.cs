using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Classification
{
    public class EamisSubClassificationDTO
    {
        public int Id { get; set; }
        public int ClassificationId { get; set; }
        public string NameSubClassification { get; set; }
        public EamisClassificationDTO ClassificationDTO { get; set; }
    }
}

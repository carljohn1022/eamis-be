using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Classification
{
    public class EamisGroupClassificationDTO
    {
        public int Id { get; set; }
        public int ClassificationId { get; set; }
        public int SubClassificationId { get; set; }
        public string NameGroupClassification { get; set; }
        public EamisClassificationDTO ClassificationDTO { get; set; }
        public EamisSubClassificationDTO SubClassificationDTO { get; set; }
    }
}

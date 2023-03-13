using EAMIS.Common.DTO.Classification;
using System;
using System.Collections.Generic;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisChartofAccountsDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string ObjectCode { get; set; }
        public string AccountCode { get; set; }
        public bool IsPartofInventroy { get; set; }
        public bool IsActive { get; set; }
        public string UserStamp { get; set; }
        public EamisGroupClassificationDTO GroupClassificationDTO { get; set; }
        public EamisClassificationDTO ClassificationDTO { get; set; }
        public EamisSubClassificationDTO SubClassificationDTO { get; set; }
    }
}

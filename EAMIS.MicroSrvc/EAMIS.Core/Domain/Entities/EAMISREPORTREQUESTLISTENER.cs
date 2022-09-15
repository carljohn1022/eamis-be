using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISREPORTREQUESTLISTENER
    {
        public int ID { get; set; }
        public string RptReqCode { get; set; }
        public string RptCode { get; set; }
        public string ParFldVal { get; set; }
        public int GenTyp { get; set; }
        public string GenRptFilNam { get; set; }
        public int RptIsReady { get; set; }
        public DateTime EntCre { get; set; }
        public string RptStatus { get; set; }
        public byte[] FileImage { get; set; }
    }
}
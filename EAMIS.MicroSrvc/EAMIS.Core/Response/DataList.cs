using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Response.DTO
{
    public class DataList<T>
    {
       
        public int Count { get; set; }
        public List<T> Items { get; set; }
        public DataList()
        {
        }
      

    }

   
}

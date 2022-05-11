using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Tweet
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string text { get; set; }
    }

}

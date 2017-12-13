using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analizator {
    class Production {


        public Production(string v) {
            var rez = v.Split(',');
            Productii = new List<string>(rez);
            Productii.Reverse();
        }
        public List<string> Productii { get; set; }
    }
    
}

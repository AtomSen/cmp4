using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analizator {
    class MyStack {
        private String str;
        public MyStack() {
            
        }
        public int Length() {
            return str.Length;
        }
        public void Push( string what) {
            str = what + str;
        }
        public string Pop() {
            var rez = str.Substring(0 , 1);
            str = str.Substring(1);
            return rez;
        }
        public string Peek(int cate=1) {
            return str.Substring(0 , cate);
        }

        internal void RemoveFromFront(string v) {
            str = str.Substring(v.Length);
        }
    }
}

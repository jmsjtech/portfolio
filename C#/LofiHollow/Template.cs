using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow {
    public class Template {
        public int TemplateID = 0;
        public string Name = "";
        public bool Prefix = false;
        public int CRmod = 0;
        public int STRbonus = 0;
        public int DEXbonus = 0;
        public int CONbonus = 0;
        public int INTbonus = 0;
        public int WISbonus = 0;
        public int CHAbonus = 0;

        public List<ClassFeature> Features = new List<ClassFeature>();
    }
}

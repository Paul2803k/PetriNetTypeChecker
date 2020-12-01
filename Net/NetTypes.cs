using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public class NetTypes
    {
        public NetTypes(string name, string type, List<String> interfaces, Declaration[] declarations) {
            this.Name = name;
            this.Type = type;
            this.Interfaces = interfaces;
            this.Declatations = declarations;
        }

        public string Name { set; get; }

        public string Type { set; get; }

        public List<String> Interfaces { set; get; }

        public Declaration[] Declatations { set; get; }
    }
}

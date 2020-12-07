using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public class NetTypes
    {
        public NetTypes(string name, string type, List<String> interfaces, List<Declaration> declarations, List<JsonTypes> originals) {
            this.Name = name;
            this.Type = type;
            this.Interfaces = interfaces;
            this.Declatations = declarations;
            this.Originals = originals;
        }

        public string Name { set; get; }

        public string Type { set; get; }

        public List<String> Interfaces { set; get; }

        public List<Declaration> Declatations { set; get; }

        public List<JsonTypes> Originals { set; get; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public class JsonTypes
    {
        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("type")]
        public string Type { set; get; }

        [JsonProperty("properties")]
        public List<Declaration> Declatations { set; get; }
    }
}

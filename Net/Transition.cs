using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public partial class Transition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("inputs")]
        public List<Inputs> Inputs { get; set; }

        [JsonProperty("outputs")]
        public List<Outputs> Outputs { get; set; }
    }
}

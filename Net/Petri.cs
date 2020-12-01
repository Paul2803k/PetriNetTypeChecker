using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public partial class Petri
    {
        [JsonProperty("places")]
        public List<Place> Places { get; set; }

        [JsonProperty("arcs")]
        public List<Arc> Arcs { get; set; }

        [JsonProperty("transitions")]
        public List<Transition> Transitions { get; set; }

        [JsonProperty("types")]
        public List<JsonTypes> Json_Types { get; set; }

        public List<NetTypes> Net_Types { get; set; }
    }
}

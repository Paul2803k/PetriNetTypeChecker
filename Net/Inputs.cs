using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public class Inputs
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public class Declaration
    {
        [JsonProperty("name")]
        public string Names { set; get; }

        [JsonProperty("type")]
        public string Type { set; get; }
    }
}

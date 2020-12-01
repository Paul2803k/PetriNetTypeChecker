using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Net
{
    public partial class Arc
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}

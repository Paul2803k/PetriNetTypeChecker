using System;
using System.Collections.Generic;
using System.Text;
using NeTypeChecker.Interfaces;
using Newtonsoft.Json;

namespace NeTypeChecker.Types
{
    class Company : IEmailable
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
    }
}

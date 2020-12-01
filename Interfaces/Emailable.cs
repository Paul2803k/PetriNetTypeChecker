using System;
using System.Collections.Generic;
using System.Text;

namespace NeTypeChecker.Interfaces
{
    public interface IEmailable
    {
        string Email { get; set; }
        string Name { get; set; }
    }
}

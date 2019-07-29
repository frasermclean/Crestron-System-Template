using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Template.Interfaces
{
    interface ITrace
    {
        bool TraceEnabled { get; set; }
        string TraceName { get; set; }
    }
}

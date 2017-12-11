using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class PropertyChangingEventArgs : CancelEventArgs
    {
        public string CancellationMessage { get; set; }
    }
}

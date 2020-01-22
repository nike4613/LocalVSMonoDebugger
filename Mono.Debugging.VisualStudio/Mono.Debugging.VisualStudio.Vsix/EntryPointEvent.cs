using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono.Debugging.VisualStudio
{
    public class EntryPointEvent : StoppingEvent
    {
        public EntryPointEvent(Thread thread) : base(thread, new Guid(IID))
        {
        }

        public const string IID = "488047F5-27E2-486C-A375-B04B7346C44C";
    }
}

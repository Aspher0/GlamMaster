using System;

namespace GlamMaster.Structs
{
    internal class GlobalEvents
    {
        public Delegate Event { get; }
        public Delegate Handler { get; }

        public GlobalEvents(Delegate evt, Delegate handler)
        {
            Event = evt;
            Handler = handler;
        }
    }
}

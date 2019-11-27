using System;

namespace CodingSeb.Mvvm
{
    public class EventToCommandArgs
    {
        public object Sender { get; set; }
        public EventArgs EventArgs { get; set; }
        public object CommandParameter { get; set; }
    }
}

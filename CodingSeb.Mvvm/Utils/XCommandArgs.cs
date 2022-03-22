using System;

namespace CodingSeb.Mvvm
{
    /// <summary>
    /// A XCommand arguments type that encaps multiple info to give to an XCommand
    /// </summary>
    public class XCommandArgs
    {
        /// <summary>
        /// The sender of a event (source)
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The args of an event source
        /// </summary>
        public EventArgs EventArgs { get; set; }

        /// <summary>
        /// The command parameter given in the XAML
        /// </summary>
        public object CommandParameter { get; set; }
    }
}

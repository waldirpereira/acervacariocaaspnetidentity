using System.Collections.Generic;

namespace Acerva.Web.Models
{
    public enum GrowlMessageSeverity
    {
        Success,
        Warning,
        Error,
        Info
    }

    public class GrowlMessage
    {
        public GrowlMessage() { }

        public GrowlMessage(GrowlMessageSeverity severity, string message, string title, List<string> details)
            : this(severity, message, title)
        {
            this.details = details;
        }

        public GrowlMessage(GrowlMessageSeverity severity, string message, string title)
            : this(severity, message)
        {
            config = new { title };
        }

        public GrowlMessage(GrowlMessageSeverity severity, string message)
        {
            this.severity = severity.ToString().ToLowerInvariant();
            this.message = message;
        }

        // ReSharper disable once InconsistentNaming
        public string message { get; set; }

        // ReSharper disable once InconsistentNaming
        public string severity { get; private set; }

        // ReSharper disable once InconsistentNaming
        public List<string> details { get; set; }

        // ReSharper disable once InconsistentNaming
        public dynamic config { get; set; }
    }
}
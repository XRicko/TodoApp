using System.Diagnostics.Tracing;

namespace ToDoList.Diagnostic
{
    [EventSource(Name = "CustomCounters.RequestTime")]
    public class ProccessTimeCounterSource : EventSource
    {
        private EventCounter eventCounter;

        public ProccessTimeCounterSource()
        {
            eventCounter = new EventCounter("request-time", this)
            {
                DisplayName = "Request Processing Time",
                DisplayUnits = "ms"
            };
        }

        public void Request(string url, float elapsedMilliseconds)
        {
            WriteEvent(1, url, elapsedMilliseconds);
            eventCounter?.WriteMetric(elapsedMilliseconds);
        }

        protected override void Dispose(bool disposing)
        {
            eventCounter?.Dispose();
            eventCounter = null;

            base.Dispose(disposing);
        }
    }
}

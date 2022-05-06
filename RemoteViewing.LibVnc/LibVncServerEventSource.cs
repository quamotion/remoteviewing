using System.Diagnostics.Tracing;
using System.Threading;

namespace RemoteViewing.LibVnc
{
    /// <summary>
    /// The event source used for <see cref="LibVncServer"/> performance counters.
    /// </summary>
    [EventSource(Name = EventSourceName)]

    public class LibVncServerEventSource : EventSource
    {
        private const string EventSourceName = "RemoteViewing-LibVncServer";

#if NET5_0_OR_GREATER
        private readonly EventCounter updateFramebufferDurationCounter;
        private readonly EventCounter processServerEventsDurationCounter;

        private readonly PollingCounter updateFramebufferCounter;
        private readonly PollingCounter processServerEventsCounter;

        private int updateFramebufferCount;
        private int processServerEventsCount;
#endif

        /// <summary>
        /// Initializes static members of the <see cref="LibVncServerEventSource"/> class.
        /// </summary>
        static LibVncServerEventSource()
        {
            Instance = new LibVncServerEventSource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibVncServerEventSource"/> class.
        /// </summary>
        public LibVncServerEventSource()
            : base(EventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat)
        {
#if NET5_0_OR_GREATER
            this.updateFramebufferDurationCounter = new EventCounter("update-framebuffer", this)
            {
                DisplayName = "Processing time for a framebuffer update",
                DisplayUnits = "ms",
            };

            this.processServerEventsDurationCounter = new EventCounter("process-serverevents", this)
            {
                DisplayName = "Processing time for server events",
                DisplayUnits = "ms",
            };

            this.updateFramebufferCounter = new PollingCounter("update-framebuffer-count", this, () => this.updateFramebufferCount)
            {
                DisplayName = "Number of framebuffer updates",
                DisplayUnits = "#",
            };

            this.processServerEventsCounter = new PollingCounter("process-serverevents-count", this, () => this.processServerEventsCount)
            {
                DisplayName = "Number of server event processing iterations",
                DisplayUnits = "#",
            };
#endif
        }

        /// <summary>
        /// Gets the default instance of the <see cref="LibVncServerEventSource"/>.
        /// </summary>
        public static LibVncServerEventSource Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Records the time required to update the framebuffer.
        /// </summary>
        /// <param name="elapsedMilliseconds">
        /// The amount of time required to update the framebuffer, in milliseconds.
        /// </param>
        public void AddFramebufferUpdateDuration(double elapsedMilliseconds)
        {
#if NET5_0_OR_GREATER
            this.updateFramebufferDurationCounter.WriteMetric(elapsedMilliseconds);
            Interlocked.Increment(ref this.updateFramebufferCount);
#endif
        }

        /// <summary>
        /// Records the time required to process server events.
        /// </summary>
        /// <param name="elapsedMilliseconds">
        /// The amount of time required to process server events, in milliseconds.
        /// </param>
        public void AddProcessServerEventsDuration(double elapsedMilliseconds)
        {
#if NET5_0_OR_GREATER
            this.processServerEventsDurationCounter.WriteMetric(elapsedMilliseconds);
            Interlocked.Increment(ref this.processServerEventsCount);
#endif
        }
    }
}

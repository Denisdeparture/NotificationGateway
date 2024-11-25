using System.Diagnostics.Metrics;

namespace NotificationService
{
    /// <summary>
    /// this interface is only needed for the tests to work correctly
    /// </summary>
    public interface IMetrics
    {
        public Counter<int>? CounterRequestsEmail { get; protected set; }

        public Counter<int>? CounterRequestsPush { get; protected set; }

        public Counter<int>? CounterRequestsSms { get; protected set; }

        public Counter<int>? CounterRequestsLogin { get; protected set; }

        public Counter<int>? CounterRequestsRegistrtion { get; protected set; }
    }
}

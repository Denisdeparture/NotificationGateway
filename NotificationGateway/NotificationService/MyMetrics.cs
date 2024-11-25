using System.Diagnostics.Metrics;

namespace NotificationService
{
    public class MyMetrics : IMetrics
    {
        public const string Name = "metrics.AspNetCoreProject";

        private int _counter = default;

        public Counter<int>? CounterRequestsEmail { get;  set; }

        public Counter<int>? CounterRequestsPush { get; set; }

        public Counter<int>? CounterRequestsSms { get; set; }

        public Counter<int>? CounterRequestsLogin { get; set; }

        public Counter<int>? CounterRequestsRegistrtion { get; set; }

        public MyMetrics(IMeterFactory factory)
        {
            ConfigMetrics(factory);
        }

        private void ConfigMetrics(IMeterFactory factory)
        {
            var meter = factory.Create(Name, "1.0.0");

            CounterRequestsEmail = meter.CreateCounter<int>(name: "Email", unit: "view_email", description: "The counter of email endpoint request");

            CounterRequestsLogin = meter.CreateCounter<int>(name: "Login", unit: "view_login", description: "The counter of login request");

            CounterRequestsRegistrtion = meter.CreateCounter<int>(name: "Registration", unit: "view_reg", description: "The counter of registration request");

            CounterRequestsSms = meter.CreateCounter<int>(name: "Sms", unit: "view_sms", description: "The counter of sms request");

            CounterRequestsPush = meter.CreateCounter<int>(name: "Push", unit: "view_push", description: "The counter of push request");

            meter.CreateObservableCounter<int>(name: "project_telemetry", observeValue: () => new Measurement<int>(_counter), unit: "project_metrics", description: "the counter request today");
        }
    }
}

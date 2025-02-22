namespace Web.Crons
{
    public class AutoReleaseSchedule : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AutoReleaseSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // từ từ chưa triển khai
                    //...
                    // quét hệ thống mỗi 30s
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }
    }
}

using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.Enums;
using Quartz;

namespace MovieBookingBackend.CRON.Jobs
{
    public class UpdateShowtimeStatusJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UpdateShowtimeStatusJob> _logger;

        public UpdateShowtimeStatusJob(IServiceProvider serviceProvider, ILogger<UpdateShowtimeStatusJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var showtimeRepository = scope.ServiceProvider.GetRequiredService<IRepository<int, Showtime>>();
                var currentDate = DateTime.Now;

                var showtimes = await showtimeRepository.GetAll();

                foreach (var showtime in showtimes)
                {
                    if(showtime.Status==ShowtimeStatus.Inactive && showtime.StartTime > currentDate)
                    {
                        showtime.Status = ShowtimeStatus.Active;
                    }
                    else if(showtime.Status == ShowtimeStatus.Active && showtime.StartTime < currentDate)
                    {
                        showtime.Status = ShowtimeStatus.Inactive;
                    }
                    await showtimeRepository.Update(showtime);
                }
                _logger.LogInformation($"Showtime Status update completed at {DateTime.Now}");
            }
        }
    }
}

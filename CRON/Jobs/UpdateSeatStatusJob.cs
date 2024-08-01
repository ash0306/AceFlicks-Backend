using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using Quartz;

namespace MovieBookingBackend.CRON.Jobs
{
    public class UpdateSeatStatusJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MovieStatusUpdateJob> _logger;

        public UpdateSeatStatusJob(IServiceProvider serviceProvider, ILogger<MovieStatusUpdateJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var seatRepository = scope.ServiceProvider.GetRequiredService<IRepository<int, Seat>>();
                var currentDate = DateTime.Now;

                var seats = (await seatRepository.GetAll()).Where(s => s.SeatStatus == SeatStatus.Reserved);

                foreach (var seat in seats)
                {
                    seat.SeatStatus = SeatStatus.Available;
                    await seatRepository.Update(seat);
                }

                _logger.LogInformation($"Seat Status update completed at {DateTime.Now}");
            }
        }
    }
}

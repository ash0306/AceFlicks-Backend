using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using Quartz;
using MovieBookingBackend.Interfaces;

namespace MovieBookingBackend.CRON.Jobs
{
    public class MovieStatusUpdateJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MovieStatusUpdateJob> _logger;

        public MovieStatusUpdateJob(IServiceProvider serviceProvider, ILogger<MovieStatusUpdateJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var movieRepository = scope.ServiceProvider.GetRequiredService<IRepository<int, Movie>>();
                var currentDate = DateTime.Now;

                var movies = await movieRepository.GetAll();

                foreach (var movie in movies)
                {
                    if(movie.Status == MovieStatus.Running && movie.EndDate < currentDate)
                    {
                        movie.Status = MovieStatus.NotRunning;
                    }
                    else if(movie.Status == MovieStatus.NotRunning && movie.StartDate <= currentDate)
                    {
                        movie.Status = MovieStatus.Running;
                    }
                    await movieRepository.Update(movie);
                }

                _logger.LogInformation($"Movie Status update completed at {DateTime.Now}");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Mappings;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;

namespace MovieBookingBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Contexts
            builder.Services.AddDbContext<MovieBookingContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
                );
            #endregion

            #region Repositories
            builder.Services.AddScoped<IRepository<int, User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int, Movie>, MovieRepository>();
            builder.Services.AddScoped<IRepository<int, Theatre>, TheatreRepository>();
            builder.Services.AddScoped<IRepository<int, Showtime>, ShowtimeRepository>();
            builder.Services.AddScoped<IRepository<int, Seat>, SeatRepository>();
            builder.Services.AddScoped<IRepository<int, Booking>, BookingRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IUserAuthService, UserAuthService>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

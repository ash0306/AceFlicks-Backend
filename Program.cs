using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.CRON;
using MovieBookingBackend.CRON.Jobs;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Mappings;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MovieBookingBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var keyVaultName = "AceTicketsVault";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            #region Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:3000", "https://agreeable-pebble-00ac1631e.5.azurestaticapps.net", "https://192.168.1.7:3000")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });
            #endregion

            #region Swagger
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            #endregion

            const string JWT = "AceTicketsJwtKey";
            var secretJwt = await client.GetSecretAsync(JWT);
            var secretToken = secretJwt.Value.Value;
            #region Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["aceTickets-token"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            #endregion

            #region Quartz
            builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
            builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            builder.Services.AddSingleton<MovieStatusUpdateJob>();
            builder.Services.AddSingleton(new JobSchedule(
                jobType: typeof(MovieStatusUpdateJob),
                cronExpression: "0 0 1 * * ?" //everyday at 1 am
                ));

            builder.Services.AddSingleton<UpdateShowtimeStatusJob>();
            builder.Services.AddSingleton(new JobSchedule(
                jobType: typeof(UpdateShowtimeStatusJob),
                cronExpression: "0 0 */1 * * ?" //every 1 hour
                ));

            builder.Services.AddSingleton<UpdateSeatStatusJob>();
            builder.Services.AddSingleton(new JobSchedule(
                jobType: typeof(UpdateSeatStatusJob),
                cronExpression: "0 */15 * * * ?" //every 15 mins
                ));

            builder.Services.AddHostedService<QuartzHostedService>();
            #endregion

            #region Logging
            builder.Services.AddLogging(l => l.AddLog4Net());
            #endregion

            #region Contexts
            const string DBsecretName = "AceTicketsdbConnectionString";
            var secret = await client.GetSecretAsync(DBsecretName);
            var connectionString = secret.Value.Value;

            builder.Services.AddDbContext<MovieBookingContext>(
                options => options.UseSqlServer(connectionString));

            #endregion

            #region Repositories
            builder.Services.AddScoped<IRepository<int, User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int, Movie>, MovieRepository>();
            builder.Services.AddScoped<IRepository<int, Theatre>, TheatreRepository>();
            builder.Services.AddScoped<IRepository<int, Showtime>, ShowtimeRepository>();
            builder.Services.AddScoped<IRepository<int, Seat>, SeatRepository>();
            builder.Services.AddScoped<IRepository<int, Booking>, BookingRepository>();
            builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            builder.Services.AddScoped<IQRCodeRepository, QRCodeRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IUserAuthService, UserAuthService>();
            builder.Services.AddScoped<IMovieServices, MovieServices>();
            builder.Services.AddScoped<ITheatreService, TheatreService>();
            builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IEmailSender, EmailSenderService>();
            builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            builder.Services.AddScoped<IQRCodeService, QRCodeService>();
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

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

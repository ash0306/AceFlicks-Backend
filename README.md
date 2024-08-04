# AceTickets-Backend

This is a comprehensive backend solution for a movie booking application. It is built using C# and provides various functionalities such as user authentication, movie management, showtime scheduling, booking services, and more.

## Table of Contents

- [Project Overview](#project-overview)
- [Functionalities](#functionalities)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
  - [User Authentication](#user-authentication)
  - [User Management](#user-management)
  - [Movie Management](#movie-management)
  - [Showtime Management](#showtime-management)
  - [Booking Management](#booking-management)
  - [Theater Management](#theater-management)
- [Installation](#installation)
- [Usage](#usage)
- [More Information](#more-information)


## Project Overview

AceTickets-Backend is designed to handle all backend operations for a movie booking system. It includes features such as user authentication, movie management, showtime scheduling, booking management, theater management, email verification, and CRON jobs for automatic updates.

## Functionalities

- **User Authentication**: Registration, login, and user management.
- **Movie Management**: Adding, updating, and deleting movies.
- **Showtime Scheduling**: Managing showtimes for different theaters.
- **Booking Management**: Handling bookings and seat allocations.
- **Theater Management**: Managing theater details and seating arrangements.
- **Email Verification**: Sending verification emails to users.
- **CRON Jobs**: Automatic updates for movie and showtime statuses.


## Project Structure
```
AceFlicks-Backend/
├── CRON/
│   ├── JobSchedule.cs
│   ├── QuartzHostedService.cs
│   ├── SingletonJobFactory.cs
│   └── Jobs/
│       ├── MovieStatusUpdateJob.cs
│       └── UpdateShowtimeStatusJob.cs
├── Contexts/
│   └── MovieBookingContext.cs
├── Controllers/
│   ├── BookingController.cs
│   ├── MovieController.cs
│   ├── ShowtimeController.cs
│   ├── TheatreController.cs
│   ├── UserAuthController.cs
│   └── UserController.cs
├── Exceptions/
│   ├── Auth/
│   ├── User/
│   ├── Booking/
│   ├── Movie/
│   ├── Theatre/
│   ├── Showtime/
│   └── Seat/
├── Models/
│   ├── DTOs/
│   └── Enums/
├── Repositories/
├── Services/
├── .gitignore
├── MovieBookingBackend.csproj
├── MovieBookingBackend.sln
├── Program.cs
└── README.md
```


## API Endpoints

### User Authentication

- **POST** `/api/auth/register`: Register a new user.
- **POST** `/api/auth/register-admin`: Register a new admin user.
- **POST** `/api/auth/login`: User login.
- **PUT** `/api/auth/password`: Update user password.
- **POST** `/api/auth/logout`: User logout.
- **POST** `/api/auth/verify/generateCode`: Generate a verification code.
- **POST** `/api/auth/verify/verifyCode/{verificationCode}`: Verify the provided verification code.

### User Management

- **GET** `/api/users`: Get all users.
- **GET** `/api/users/customers`: Get all customer users.
- **GET** `/api/users/admins`: Get all admin users.
- **GET** `/api/users/id/{id}`: Get user by ID.
- **GET** `/api/users/email/{email}`: Get user by email.
- **PUT** `/api/users`: Update user details.

### Movie Management

- **POST** `/api/movies`: Add a new movie.
- **GET** `/api/movies`: Get all movies.
- **GET** `/api/movies/running`: Get running movies.
- **GET** `/api/movies/byLanguages`: Get movies by languages.
- **PUT** `/api/movies`: Update movie details.
- **GET** `/api/movies/movie/{movieName}`: Get movie by name.

### Showtime Management

- **POST** `/api/showtimes`: Add a new showtime.
- **GET** `/api/showtimes`: Get all showtimes.
- **GET** `/api/showtimes/{id}`: Get showtime by ID.
- **PUT** `/api/showtimes/status`: Update showtime status.
- **GET** `/api/showtimes/theatre/{theatreName}`: Get showtimes by theater name.
- **GET** `/api/showtimes/seats/{showtimeId}`: Get seats for a showtime.

### Booking Management

- **POST** `/api/bookings`: Add a new booking.
- **GET** `/api/bookings`: Get all bookings.
- **GET** `/api/bookings/{id}`: Get booking by ID.

### Theater Management

- **POST** `/api/theatres`: Add a new theater.
- **GET** `/api/theatres`: Get all theaters.
- **GET** `/api/theatres/{id}`: Get theater by ID.
- **GET** `/api/theatres/locations`: Get all theater locations.
- **PUT** `/api/theatres`: Update theater details.

## Installation

To set up the project locally, follow these steps:

1. Clone the repository:

    ```bash
    git clone https://github.com/yourusername/AceTickets-Backend.git
    cd AceTickets-Backend
    ```

2. Update `appsettings.json` with your SQL Server connection string and SMTP settings.

3. Run the migrations and update the database:

    ```bash
    dotnet ef database update
    ```

4. Run the backend application:

    ```bash
    dotnet run
    ```

## Usage

Once the application is set up, you can use the provided API endpoints to manage users, movies, showtimes, bookings, and theaters.

## More information

For more information on this project, plase refer [here](https://github.com/ash0306/Capstone-Project-Genspark)
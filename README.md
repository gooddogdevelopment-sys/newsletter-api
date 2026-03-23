# Dotnet Core API Template with Postgres

## Overview
This project is a template for building .NET Core applications backed by a PostgreSQL database. It serves as a starting point for developing robust APIs with modern practices.

## Features
- **ASP.NET Core** for building web APIs
- **Entity Framework Core** for data access
- **PostgreSQL** as the database provider
- **Swagger** for API documentation
- **JWT Authentication** for secure access

## Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/gooddogdevelopment-sys/dotnet-core-api-template-w-postgres.git
   cd dotnet-core-api-template-w-postgres
   ```
2. **Configure the database**:
   - Create a PostgreSQL database and obtain the connection string.
   - Update the `appsettings.json` file with your connection string:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=your_database;Username=your_username;Password=your_password"
   }
   ```
3. **Install dependencies**:
   ```bash
   dotnet restore
   ```
4. **Run migrations**:
   ```bash
   dotnet ef database update
   ```
5. **Start the application**:
   ```bash
   dotnet run
   ```

## Usage
- The API can be accessed at `http://localhost:5000`
- Use Swagger UI available at `http://localhost:5000/swagger` to explore the API endpoints and test them interactively.

## Contributing
If you'd like to contribute, feel free to fork the repository and submit a pull request. 

## License
This project is licensed under the MIT License.
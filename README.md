# Articulus

A modern, feature-rich article management and sharing platform built with ASP.NET Core 9.0. Articulus provides a comprehensive solution for creating, managing, and interacting with articles, with integrated news aggregation and social features.

## 🚀 Features

### Article Management
- **Create & Edit Articles**: Write and publish articles with rich content and cover images
- **Article Interactions**: Like, bookmark, and share articles
- **Comments System**: Comment on articles with support for nested discussions
- **Comment Voting**: Upvote or downvote comments
- **Reactions**: React to articles with various emotions
- **Article Reporting**: Report inappropriate content

### User Features
- **User Authentication**: Secure registration and login with JWT-based authentication
- **Email Verification**: OTP-based email verification during registration
- **User Profiles**: Customizable user profiles with bio and profile pictures
- **Follow System**: Follow other users to stay updated with their content
- **User Actions**: Track user activity including views, bookmarks, and shares

### News Integration
- **News Aggregation**: Fetch and display latest news from external sources
- **News API Integration**: Integrated with NewsAPI for real-time news updates

### Security & Monitoring
- **JWT Authentication**: Secure API endpoints with JSON Web Tokens
- **Global Exception Handling**: Comprehensive error handling across the application
- **Logging**: Structured logging with Serilog and Seq integration
- **Action Logging**: Automatic logging of all API actions

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Language**: C# with .NET 9.0
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: JWT Bearer Authentication
- **API Documentation**: Swagger/OpenAPI

### Key Libraries & Packages
- **Entity Framework Core 9.0.10**: ORM for database operations
- **Serilog 9.0.0**: Structured logging
- **Swashbuckle.AspNetCore 9.0.4**: Swagger/OpenAPI documentation
- **System.IdentityModel.Tokens.Jwt 8.14.0**: JWT token handling
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0.9**: JWT authentication

## 📁 Project Architecture

The solution follows a clean architecture pattern with separation of concerns:

```
Articulus/
├── Articulus/                  # Main API project
│   ├── Controllers/           # API Controllers
│   │   ├── Articles/         # Article-related endpoints
│   │   ├── News/            # News endpoints
│   │   └── Users/           # User & auth endpoints
│   ├── Filters/             # Action filters (logging, exceptions)
│   └── Program.cs           # Application entry point
├── Articulus.BLL/            # Business Logic Layer
│   ├── Articles/            # Article business logic
│   ├── News/               # News business logic
│   ├── Users/              # User business logic
│   └── Exceptions/         # Custom exceptions
├── Articulus.Data/           # Data Access Layer
│   ├── Models/             # Entity models
│   ├── Migrations/         # EF Core migrations
│   └── AppDbContext.cs     # Database context
├── Articulus.DTOs/           # Data Transfer Objects
│   ├── Articles/           # Article DTOs
│   ├── Auth/               # Authentication DTOs
│   ├── Users/              # User DTOs
│   └── Env/                # Environment settings
└── Articulus.Services/       # External Services
    ├── JwtService          # JWT token generation
    └── MailService         # Email service
```

## 📋 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- (Optional) [Seq](https://datalust.co/seq) for log aggregation

## 🔧 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Nitesh-888/Articulus.git
   cd Articulus
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update the database connection string**
   
   Edit `Articulus/appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=Articulus;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

4. **Configure application settings**
   
   Create `Articulus/appsettings.Development.json` with your settings:
   ```json
   {
     "Jwt": {
       "Key": "YOUR_SECRET_KEY_HERE_AT_LEAST_32_CHARACTERS",
       "Issuer": "YourIssuer",
       "Audience": "YourAudience"
     },
     "MailSettings": {
       "SmtpServer": "smtp.gmail.com",
       "Port": 587,
       "SenderName": "Articulus",
       "SenderEmail": "your-email@example.com",
       "Password": "your-app-password"
     },
     "NewsApi": {
       "ApiKey": "YOUR_NEWSAPI_KEY"
     }
   }
   ```

5. **Apply database migrations**
   ```bash
   cd Articulus
   dotnet ef database update
   ```

## 🚀 Running the Application

1. **Run the application**
   ```bash
   dotnet run --project Articulus
   ```

2. **Access the application**
   - API: `https://localhost:5001` or `http://localhost:5000`
   - Swagger UI: `https://localhost:5001/swagger` (in Development mode)

## 📚 API Documentation

Once the application is running in Development mode, you can access the interactive API documentation at:
- Swagger UI: `https://localhost:5001/swagger`
- OpenAPI JSON: `https://localhost:5001/swagger/v1/swagger.json`

### Main API Endpoints

#### Authentication
- `POST /api/Auth/Register` - Register a new user
- `POST /api/Auth/Register/Verify` - Verify email with OTP
- `POST /api/Auth/Login` - Login and receive JWT token

#### Articles
- `GET /api/Articles` - Get all articles
- `GET /api/Articles/{id}` - Get article by ID
- `POST /api/Articles` - Create a new article
- `PUT /api/Articles/{id}` - Update an article
- `DELETE /api/Articles/{id}` - Delete an article

#### Article Interactions
- `POST /api/ArticleComments` - Add a comment
- `POST /api/ArticleReactions` - React to an article
- `POST /api/ArticleActions/bookmark` - Bookmark an article
- `POST /api/AritcleCommentVotes` - Vote on a comment

#### User Profile
- `GET /api/UserProfile/{userId}` - Get user profile
- `PUT /api/UserProfile` - Update user profile
- `POST /api/UserActions/follow` - Follow a user

#### News
- `GET /api/News` - Get latest news articles

## 🔐 Authentication

The API uses JWT (JSON Web Token) based authentication. To access protected endpoints:

1. Register a new user via `/api/Auth/Register`
2. Verify your email via `/api/Auth/Register/Verify`
3. Login via `/api/Auth/Login` to receive a JWT token
4. Include the token in the `Authorization` header for subsequent requests:
   ```
   Authorization: Bearer YOUR_JWT_TOKEN
   ```

## 🗄️ Database Schema

The application uses SQL Server with Entity Framework Core. Key entities include:

- **User**: User accounts and profiles
- **UserCred**: User credentials and authentication
- **Article**: Published articles
- **Comment**: Article comments
- **Reaction**: Article reactions
- **CommentVote**: Comment votes
- **Bookmark**: Bookmarked articles
- **UserFollow**: User follow relationships
- **View**: Article view tracking
- **Report**: Content reports
- **Feedback**: User feedback

## 🔍 Logging

The application uses Serilog for structured logging with:
- Console sink for development
- Seq sink for log aggregation (http://localhost:5341)

All API actions are automatically logged via the `LogActionFilter`.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👨‍💻 Author

**Nitesh-888**
- GitHub: [@Nitesh-888](https://github.com/Nitesh-888)

## 🙏 Acknowledgments

- Thanks to all contributors who have helped this project grow
- Built with ASP.NET Core and Entity Framework Core
- News powered by [NewsAPI](https://newsapi.org/)

---

For issues, questions, or suggestions, please open an issue on the [GitHub repository](https://github.com/Nitesh-888/Articulus/issues).

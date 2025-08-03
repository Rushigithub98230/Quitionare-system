# Backend Extraction Guide - .NET Core Questionnaire System

This guide provides detailed instructions for extracting the .NET Core backend from the Questionnaire System to use in another application.

## 📁 Project Structure

```
src/
├── QuestionnaireSystem.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CategoriesController.cs
│   │   ├── CategoryQuestionnaireTemplatesController.cs
│   │   ├── QuestionTypesController.cs
│   │   ├── ResponsesController.cs
│   │   └── UploadController.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── CategoryQuestionnaireTemplateService.cs
│   │   ├── CategoryService.cs
│   │   ├── JwtService.cs
│   │   ├── QuestionTypeService.cs
│   │   └── UserQuestionResponseService.cs
│   ├── Helpers/
│   │   └── TokenHelper.cs
│   ├── Mapping/
│   │   └── MappingProfile.cs
│   ├── Validators/
│   │   └── CreateCategoryDtoValidator.cs
│   ├── Migrations/
│   │   └── [Database Migration Files]
│   ├── Program.cs
│   ├── appsettings.json
│   └── QuestionnaireSystem.API.csproj
├── QuestionnaireSystem.Core/
│   ├── DTOs/
│   │   ├── AnalyticsDto.cs
│   │   ├── AuthDto.cs
│   │   ├── CategoryDto.cs
│   │   ├── CategoryQuestionDto.cs
│   │   ├── CategoryQuestionnaireTemplateDto.cs
│   │   ├── QuestionOptionDto.cs
│   │   ├── QuestionResponseDto.cs
│   │   ├── QuestionTypeDto.cs
│   │   ├── ResponseDto.cs
│   │   └── UserQuestionResponseDto.cs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ICategoryQuestionnaireTemplateRepository.cs
│   │   ├── ICategoryQuestionnaireTemplateService.cs
│   │   ├── ICategoryQuestionRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ICategoryService.cs
│   │   ├── IJwtService.cs
│   │   ├── IQuestionTypeRepository.cs
│   │   ├── IQuestionTypeService.cs
│   │   ├── IUserQuestionResponseRepository.cs
│   │   └── IUserQuestionResponseService.cs
│   ├── Models/
│   │   ├── AdminConstants.cs
│   │   ├── Category.cs
│   │   ├── CategoryQuestion.cs
│   │   ├── CategoryQuestionnaireTemplate.cs
│   │   ├── JsonModel.cs
│   │   ├── QuestionOption.cs
│   │   ├── QuestionOptionResponse.cs
│   │   ├── QuestionResponse.cs
│   │   ├── QuestionType.cs
│   │   ├── TokenModel.cs
│   │   ├── User.cs
│   │   └── UserQuestionResponse.cs
│   └── QuestionnaireSystem.Core.csproj
├── QuestionnaireSystem.Infrastructure/
│   ├── Data/
│   │   └── QuestionnaireDbContext.cs
│   ├── Repositories/
│   │   ├── CategoryQuestionnaireTemplateRepository.cs
│   │   ├── CategoryQuestionRepository.cs
│   │   ├── CategoryRepository.cs
│   │   ├── QuestionTypeRepository.cs
│   │   └── UserQuestionResponseRepository.cs
│   └── QuestionnaireSystem.Infrastructure.csproj
└── QuestionnaireSystem.API.Tests/
    ├── QuestionTypeChangeTests.cs
    ├── ValidationAndEdgeCaseTests.cs
    └── QuestionnaireSystem.API.Tests.csproj
```

## 🚀 Extraction Steps

### Step 1: Copy Core Project Files

#### 1.1 Copy the entire `src/` directory
```bash
# Copy the entire src folder to your new project
cp -r src/ /path/to/your/new/project/
```

#### 1.2 Essential Files to Copy
- `src/QuestionnaireSystem.API/` - Main API project
- `src/QuestionnaireSystem.Core/` - Domain models and interfaces
- `src/QuestionnaireSystem.Infrastructure/` - Data access layer
- `src/QuestionnaireSystem.API.Tests/` - Unit tests (optional)

### Step 2: Update Project Files

#### 2.1 Update `QuestionnaireSystem.API.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\QuestionnaireSystem.Core\QuestionnaireSystem.Core.csproj" />
    <ProjectReference Include="..\QuestionnaireSystem.Infrastructure\QuestionnaireSystem.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

#### 2.2 Update `QuestionnaireSystem.Core.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
  </ItemGroup>
</Project>
```

#### 2.3 Update `QuestionnaireSystem.Infrastructure.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="AutoMapper" Version="12.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\QuestionnaireSystem.Core\QuestionnaireSystem.Core.csproj" />
  </ItemGroup>
</Project>
```

### Step 3: Update Configuration Files

#### 3.1 Update `appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "your-application-name",
    "Audience": "your-application-users",
    "ExpirationHours": 24
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://your-frontend-domain.com"
    ]
  }
}
```

#### 3.2 Update `appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName_Dev;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Step 4: Update Program.cs

#### 4.1 Update `Program.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestionnaireSystem.API.Mapping;
using QuestionnaireSystem.API.Services;
using QuestionnaireSystem.Core.Interfaces;
using QuestionnaireSystem.Infrastructure.Data;
using QuestionnaireSystem.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins(builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? new string[0])
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

// Add DbContext
builder.Services.AddDbContext<QuestionnaireDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Services
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryQuestionnaireTemplateRepository, CategoryQuestionnaireTemplateRepository>();
builder.Services.AddScoped<ICategoryQuestionnaireTemplateService, CategoryQuestionnaireTemplateService>();
builder.Services.AddScoped<ICategoryQuestionRepository, CategoryQuestionRepository>();
builder.Services.AddScoped<IQuestionTypeRepository, QuestionTypeRepository>();
builder.Services.AddScoped<IQuestionTypeService, QuestionTypeService>();
builder.Services.AddScoped<IUserQuestionResponseRepository, UserQuestionResponseRepository>();
builder.Services.AddScoped<IUserQuestionResponseService, UserQuestionResponseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not found"))
            )
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 5: Update Database Context

#### 5.1 Update `QuestionnaireDbContext.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Infrastructure.Data;

public class QuestionnaireDbContext : DbContext
{
    public QuestionnaireDbContext(DbContextOptions<QuestionnaireDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryQuestion> Questions { get; set; }
    public DbSet<CategoryQuestionnaireTemplate> CategoryQuestionnaireTemplates { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserQuestionResponse> UserQuestionResponses { get; set; }
    public DbSet<QuestionResponse> QuestionResponses { get; set; }
    public DbSet<QuestionOptionResponse> QuestionOptionResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure your entity relationships and constraints here
        // Example:
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .HasIndex(q => q.Title)
            .IsUnique();
    }
}
```

### Step 6: Update Models and DTOs

#### 6.1 Update model files to match your business requirements
- Update property names and types
- Add new properties as needed
- Remove unused properties
- Update validation attributes

#### 6.2 Update DTOs to match your API requirements
- Modify DTO structures
- Update property mappings
- Add new DTOs as needed

### Step 7: Update Controllers

#### 7.1 Update controller namespaces and routing
```csharp
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    // Update controller logic as needed
}
```

#### 7.2 Update authentication and authorization
```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    // Add authentication logic as needed
}
```

### Step 8: Update Services

#### 8.1 Update service implementations
- Modify business logic
- Update validation rules
- Add new functionality
- Remove unused methods

#### 8.2 Update repository implementations
- Modify data access logic
- Update query optimizations
- Add new repository methods

## 🔧 Setup Instructions for New Project

### 1. Create New Solution
```bash
# Create new solution
dotnet new sln -n YourApplicationName

# Add projects to solution
dotnet sln add src/QuestionnaireSystem.API/QuestionnaireSystem.API.csproj
dotnet sln add src/QuestionnaireSystem.Core/QuestionnaireSystem.Core.csproj
dotnet sln add src/QuestionnaireSystem.Infrastructure/QuestionnaireSystem.Infrastructure.csproj
```

### 2. Install Dependencies
```bash
# Install packages for API project
cd src/QuestionnaireSystem.API
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore

# Install packages for Infrastructure project
cd ../QuestionnaireSystem.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package AutoMapper

# Install packages for Core project
cd ../QuestionnaireSystem.Core
dotnet add package System.ComponentModel.Annotations
```

### 3. Update Database Connection
```bash
# Update connection string in appsettings.json
# Then create and apply migrations
cd src/QuestionnaireSystem.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Update Namespaces
Update all namespace declarations in:
- Controllers
- Services
- Repositories
- Models
- DTOs

### 5. Update Application Name
- Update project names in `.csproj` files
- Update namespace declarations
- Update solution name
- Update API documentation

## 🎨 Customization Options

### 1. Database Configuration
- Update connection strings
- Modify entity configurations
- Add new entities
- Update relationships

### 2. Authentication & Authorization
- Implement custom authentication
- Add role-based authorization
- Update JWT configuration
- Add custom claims

### 3. API Endpoints
- Add new controllers
- Modify existing endpoints
- Update routing
- Add new DTOs

### 4. Business Logic
- Update service implementations
- Add new business rules
- Modify validation logic
- Add new features

## 🔍 Key Components to Customize

### 1. Models and DTOs
- Update entity properties
- Add new entities
- Modify relationships
- Update validation rules

### 2. Controllers
- Update endpoint logic
- Add new endpoints
- Modify authentication
- Update error handling

### 3. Services
- Update business logic
- Add new services
- Modify validation
- Update data processing

### 4. Repositories
- Update data access logic
- Add new queries
- Optimize performance
- Add caching

## 🚨 Important Notes

### 1. Database Migration
- Create new migrations for your schema changes
- Test migrations thoroughly
- Backup existing data before applying migrations
- Consider data seeding for development

### 2. Security Configuration
- Update JWT settings
- Configure CORS properly
- Implement proper authentication
- Add authorization policies

### 3. Environment Configuration
- Set up different environments (Development, Staging, Production)
- Configure connection strings for each environment
- Set up proper logging
- Configure error handling

### 4. API Documentation
- Update Swagger documentation
- Add proper API descriptions
- Include example requests/responses
- Document authentication requirements

## 📝 Testing Checklist

- [ ] All controllers respond correctly
- [ ] Database operations work properly
- [ ] Authentication flows correctly
- [ ] Authorization works as expected
- [ ] API documentation is accurate
- [ ] Error handling works properly
- [ ] Validation rules are enforced
- [ ] Performance is acceptable
- [ ] Security measures are in place

## 🆘 Troubleshooting

### Common Issues:

1. **Database connection errors**
   - Verify connection string
   - Check SQL Server is running
   - Verify database exists
   - Check user permissions

2. **Migration errors**
   - Check for conflicting migrations
   - Verify model changes
   - Check for data type conflicts
   - Verify foreign key relationships

3. **Authentication errors**
   - Verify JWT configuration
   - Check secret key length
   - Verify issuer/audience settings
   - Check token expiration

4. **CORS errors**
   - Verify CORS configuration
   - Check allowed origins
   - Verify frontend URL
   - Check credentials settings

## 📞 Support

If you encounter issues during extraction:
1. Check the original project structure
2. Verify all dependencies are correctly installed
3. Ensure database configuration is correct
4. Test API endpoints individually
5. Review application logs for specific errors

## 🔄 Migration Steps

### 1. Create New Database
```sql
CREATE DATABASE YourDatabaseName;
```

### 2. Update Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=YourDatabaseName;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Create Initial Migration
```bash
dotnet ef migrations add InitialCreate
```

### 4. Apply Migration
```bash
dotnet ef database update
```

### 5. Seed Initial Data (if needed)
```csharp
// Add seeding logic in Program.cs or create a separate seeding service
```

This extraction guide should help you successfully move the .NET Core backend to your new application while maintaining all functionality and customizing it to your specific needs. 
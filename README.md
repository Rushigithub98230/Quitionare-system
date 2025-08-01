# 📋 Questionnaire System - Comprehensive Guide

## 🎯 System Overview

This is a **subscription-based healthcare questionnaire system** that allows dynamic question creation and response collection. Users are assigned to categories (like "Hair Loss", "Weight Loss") and can access questionnaires based on their subscription category.

## 🏗️ Architecture

### **Technology Stack:**
- **Backend:** .NET 8 Web API with Entity Framework Core
- **Frontend:** Angular 17 with Material Design
- **Database:** SQL Server with Entity Framework migrations
- **Authentication:** JWT-based authentication

### **Core Principles:**
- **Clean Architecture** - Separation of concerns
- **Consistent Response Format** - All APIs return JsonModel
- **Token-Based Authentication** - Every service method includes TokenModel
- **Dynamic Question Creation** - Admin can create any type of question
- **Category-Based Access** - Users see questionnaires based on their category

## 📊 Database Schema

### **Core Entities:**

#### **1. Categories (Subscription Categories)**
```sql
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL, -- e.g., "Hair Loss", "Weight Loss"
    Description NVARCHAR(500),
    Color NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    DeletedAt DATETIME2 NULL
);
```

#### **2. Users**
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(50) DEFAULT 'User',
    Category NVARCHAR(100), -- e.g., "Hair Loss", "Weight Loss"
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    LastLoginAt DATETIME2 NULL
);
```

#### **3. QuestionTypes (Predefined)**
```sql
CREATE TABLE QuestionTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TypeName NVARCHAR(50) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    HasOptions BIT DEFAULT 0,
    SupportsFileUpload BIT DEFAULT 0,
    SupportsImage BIT DEFAULT 0,
    IsActive BIT DEFAULT 1
);
```

#### **4. QuestionnaireTemplates**
```sql
CREATE TABLE QuestionnaireTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    IsActive BIT DEFAULT 1,
    IsMandatory BIT DEFAULT 0,
    DisplayOrder INT DEFAULT 0,
    Version INT DEFAULT 1,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    DeletedAt DATETIME2 NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);
```

#### **5. Questions**
```sql
CREATE TABLE Questions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    QuestionnaireId UNIQUEIDENTIFIER NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionTypeId UNIQUEIDENTIFIER NOT NULL,
    IsRequired BIT DEFAULT 0,
    DisplayOrder INT NOT NULL,
    SectionName NVARCHAR(100),
    HelpText NVARCHAR(MAX),
    Placeholder NVARCHAR(255),
    MinLength INT,
    MaxLength INT,
    MinValue DECIMAL(18,2),
    MaxValue DECIMAL(18,2),
    ImageUrl NVARCHAR(MAX),
    ImageAltText NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    DeletedAt DATETIME2 NULL,
    FOREIGN KEY (QuestionnaireId) REFERENCES QuestionnaireTemplates(Id),
    FOREIGN KEY (QuestionTypeId) REFERENCES QuestionTypes(Id)
);
```

#### **6. QuestionOptions**
```sql
CREATE TABLE QuestionOptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    OptionText NVARCHAR(500) NOT NULL,
    OptionValue NVARCHAR(255) NOT NULL,
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    HasTextInput BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
);
```

#### **7. UserQuestionResponses**
```sql
CREATE TABLE UserQuestionResponses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    QuestionnaireId UNIQUEIDENTIFIER NOT NULL,
    StartedAt DATETIME2 DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    IsCompleted BIT DEFAULT 0,
    IsDraft BIT DEFAULT 1,
    SubmissionIp NVARCHAR(45),
    UserAgent NVARCHAR(MAX),
    TimeTaken INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (QuestionnaireId) REFERENCES QuestionnaireTemplates(Id)
);
```

#### **8. QuestionResponses**
```sql
CREATE TABLE QuestionResponses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ResponseId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    TextResponse NVARCHAR(MAX),
    NumberResponse DECIMAL(18,2),
    DateResponse DATETIME2,
    DatetimeResponse DATETIME2,
    BooleanResponse BIT,
    JsonResponse NVARCHAR(MAX),
    FilePath NVARCHAR(500),
    FileName NVARCHAR(255),
    FileSize INT,
    FileType NVARCHAR(100),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ResponseId) REFERENCES UserQuestionResponses(Id),
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
);
```

#### **9. QuestionOptionResponses**
```sql
CREATE TABLE QuestionOptionResponses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    QuestionResponseId UNIQUEIDENTIFIER NOT NULL,
    OptionId UNIQUEIDENTIFIER NOT NULL,
    CustomText NVARCHAR(1000),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (QuestionResponseId) REFERENCES QuestionResponses(Id),
    FOREIGN KEY (OptionId) REFERENCES QuestionOptions(Id)
);
```

## 🎯 Supported Question Types

| ID | Type Name | Display Name | Has Options | File Upload | Description |
|----|-----------|--------------|-------------|-------------|-------------|
| 1 | `text` | Text Input | ❌ | ❌ | Single line text input |
| 2 | `textarea` | Text Area | ❌ | ❌ | Multi-line text input |
| 3 | `radio` | Radio Button | ✅ | ❌ | Single choice selection |
| 4 | `checkbox` | Checkbox | ✅ | ❌ | Multiple choice selection |
| 5 | `select` | Dropdown | ✅ | ❌ | Single choice dropdown |
| 6 | `multiselect` | Multi-Select | ✅ | ❌ | Multiple choice dropdown |
| 7 | `number` | Number | ❌ | ❌ | Numeric input |
| 8 | `date` | Date | ❌ | ❌ | Date picker |
| 9 | `email` | Email | ❌ | ❌ | Email input with validation |
| 10 | `phone` | Phone | ❌ | ❌ | Phone number input |
| 11 | `file` | File Upload | ❌ | ✅ | File/document upload |
| 12 | `rating` | Rating Scale | ❌ | ❌ | Star rating (1-5) |
| 13 | `slider` | Slider | ❌ | ❌ | Range slider input |
| 14 | `yes_no` | Yes/No | ✅ | ❌ | Boolean choice |

## 🔄 Data Flow

### **1. Question Creation Flow:**
```
Admin → Creates Category → Creates Questionnaire → Adds Questions → Adds Options (if needed)
```

### **2. User Response Flow:**
```
User Login → Select Category → View Questionnaires → Start Response → Submit Answers → Store in Database
```

### **3. Data Storage Examples:**

#### **A. Text Question Creation:**
```sql
-- Create question
INSERT INTO Questions (QuestionnaireId, QuestionText, QuestionTypeId, IsRequired, MinLength, MaxLength)
VALUES ('questionnaire-id', 'What is your name?', 'text-type-id', 1, 2, 50);

-- User response
INSERT INTO QuestionResponses (ResponseId, QuestionId, TextResponse)
VALUES ('response-id', 'question-id', 'John Doe');
```

#### **B. Radio Question Creation:**
```sql
-- Create question
INSERT INTO Questions (QuestionnaireId, QuestionText, QuestionTypeId, IsRequired)
VALUES ('questionnaire-id', 'What is your gender?', 'radio-type-id', 1);

-- Create options
INSERT INTO QuestionOptions (QuestionId, OptionText, OptionValue, DisplayOrder)
VALUES 
('question-id', 'Male', 'male', 1),
('question-id', 'Female', 'female', 2),
('question-id', 'Other', 'other', 3);

-- User response
INSERT INTO QuestionResponses (ResponseId, QuestionId, TextResponse)
VALUES ('response-id', 'question-id', 'male');
INSERT INTO QuestionOptionResponses (QuestionResponseId, OptionId)
VALUES ('question-response-id', 'male-option-id');
```

#### **C. File Upload Question:**
```sql
-- Create question
INSERT INTO Questions (QuestionnaireId, QuestionText, QuestionTypeId, IsRequired)
VALUES ('questionnaire-id', 'Upload your photo', 'file-type-id', 1);

-- User response
INSERT INTO QuestionResponses (ResponseId, QuestionId, FilePath, FileName, FileSize, FileType)
VALUES ('response-id', 'question-id', '/uploads/photo.jpg', 'photo.jpg', 1024000, 'image/jpeg');
```

## 🏛️ Architecture Components

### **1. Controllers (Clean & Simple)**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuestionnairesController : ControllerBase
{
    private readonly IQuestionnaireService _questionnaireService;

    public QuestionnairesController(IQuestionnaireService questionnaireService)
    {
        _questionnaireService = questionnaireService;
    }

    [HttpGet]
    public async Task<ActionResult<JsonModel>> GetAll()
    {
        return await _questionnaireService.GetAllAsync(GetToken(HttpContext));
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> Create(CreateQuestionnaireDto dto)
    {
        return await _questionnaireService.CreateAsync(dto, GetToken(HttpContext));
    }
}
```

### **2. Services (Business Logic)**
```csharp
public class QuestionnaireService : IQuestionnaireService
{
    private readonly IQuestionnaireRepository _repository;
    private readonly IMapper _mapper;

    public QuestionnaireService(IQuestionnaireRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<JsonModel> GetAllAsync(TokenModel tokenModel)
    {
        try
        {
            var questionnaires = await _repository.GetAllAsync();
            return JsonModel.SuccessResult(questionnaires, "Questionnaires retrieved successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error retrieving questionnaires: {ex.Message}");
        }
    }

    public async Task<JsonModel> CreateAsync(CreateQuestionnaireDto dto, TokenModel tokenModel)
    {
        try
        {
            // Validate user permissions
            if (tokenModel.Role != "Admin")
                return JsonModel.ErrorResult("Access denied", HttpStatusCodes.Forbidden);

            var questionnaire = _mapper.Map<QuestionnaireTemplate>(dto);
            questionnaire.CreatedBy = tokenModel.UserId;
            
            var result = await _repository.CreateAsync(questionnaire);
            return JsonModel.SuccessResult(result, "Questionnaire created successfully");
        }
        catch (Exception ex)
        {
            return JsonModel.ErrorResult($"Error creating questionnaire: {ex.Message}");
        }
    }
}
```

### **3. Repositories (Data Access)**
```csharp
public class QuestionnaireRepository : IQuestionnaireRepository
{
    private readonly QuestionnaireDbContext _context;

    public QuestionnaireRepository(QuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuestionnaireTemplate>> GetAllAsync()
    {
        return await _context.QuestionnaireTemplates
            .Include(qt => qt.Category)
            .Include(qt => qt.Questions.OrderBy(q => q.DisplayOrder))
                .ThenInclude(q => q.QuestionType)
            .Include(qt => qt.Questions)
                .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
            .Where(qt => qt.IsActive && qt.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<QuestionnaireTemplate> CreateAsync(QuestionnaireTemplate questionnaire)
    {
        _context.QuestionnaireTemplates.Add(questionnaire);
        await _context.SaveChangesAsync();
        return questionnaire;
    }
}
```

## 🔐 Authentication & Authorization

### **TokenModel Structure:**
```csharp
public class TokenModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
```

### **JWT Token Helper:**
```csharp
public static class TokenHelper
{
    public static TokenModel GetToken(HttpContext httpContext)
    {
        var user = httpContext.User;
        return new TokenModel
        {
            UserId = Guid.Parse(user.FindFirst("UserId")?.Value ?? Guid.Empty.ToString()),
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            Category = user.FindFirst("Category")?.Value ?? string.Empty
        };
    }
}
```

## 📋 API Endpoints

### **Categories:**
- `GET /api/categories` - Get all categories
- `POST /api/categories` - Create category (Admin only)
- `PUT /api/categories/{id}` - Update category (Admin only)
- `DELETE /api/categories/{id}` - Delete category (Admin only)

### **Questionnaires:**
- `GET /api/questionnaires` - Get user's questionnaires (by category)
- `GET /api/questionnaires/{id}` - Get questionnaire details
- `POST /api/questionnaires` - Create questionnaire (Admin only)
- `PUT /api/questionnaires/{id}` - Update questionnaire (Admin only)
- `DELETE /api/questionnaires/{id}` - Delete questionnaire (Admin only)

### **Responses:**
- `GET /api/responses` - Get user's responses
- `GET /api/responses/{id}` - Get response details
- `POST /api/responses` - Submit response
- `PUT /api/responses/{id}` - Update response

### **Auth:**
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Refresh token

## 🎯 Response Format

### **JsonModel Structure:**
```json
{
    "data": {},
    "message": "Success",
    "statusCode": 200,
    "success": true,
    "timestamp": "2024-01-15T10:30:00Z"
}
```

### **Success Response:**
```json
{
    "data": {
        "id": "12345678-1234-1234-1234-123456789012",
        "title": "Hair Loss Assessment",
        "description": "Complete hair loss evaluation",
        "categoryId": "category-id",
        "questions": []
    },
    "message": "Questionnaire retrieved successfully",
    "statusCode": 200,
    "success": true,
    "timestamp": "2024-01-15T10:30:00Z"
}
```

### **Error Response:**
```json
{
    "data": null,
    "message": "Questionnaire not found",
    "statusCode": 404,
    "success": false,
    "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🚀 Development Guidelines

### **1. Controller Guidelines:**
- Keep controllers **thin** - only call services
- Always use `TokenModel` from `GetToken(HttpContext)`
- Return `JsonModel` from all endpoints
- Handle exceptions at service level

### **2. Service Guidelines:**
- All service methods must include `TokenModel tokenModel` parameter
- Always return `JsonModel`
- Implement proper validation
- Handle business logic and exceptions

### **3. Repository Guidelines:**
- Use Entity Framework Core
- Implement proper LINQ queries
- Use Include() for related data
- Implement soft delete where needed

### **4. Database Guidelines:**
- Use soft delete (DeletedAt) for main entities
- Implement proper indexes
- Use UTC timestamps
- Follow naming conventions

## 🔧 Setup Instructions

### **1. Database Setup:**
```bash
# Run migrations
dotnet ef database update

# Seed data
dotnet run --seed
```

### **2. API Setup:**
```bash
# Install dependencies
dotnet restore

# Run the application
dotnet run
```

### **3. Frontend Setup:**
```bash
# Install dependencies
npm install

# Run development server
ng serve
```

## 📊 Sample Data

### **Categories:**
```sql
INSERT INTO Categories (Name, Description, Color) VALUES 
('Hair Loss', 'Hair loss treatment and monitoring', '#8B4513'),
('Weight Loss', 'Weight loss and fitness tracking', '#32CD32'),
('Skin Care', 'Skin condition and treatment', '#FFB6C1'),
('Mental Health', 'Mental health and wellness', '#9370DB');
```

### **Question Types:**
```sql
INSERT INTO QuestionTypes (TypeName, DisplayName, HasOptions) VALUES 
('text', 'Text Input', 0),
('radio', 'Single Choice', 1),
('checkbox', 'Multiple Choice', 1),
('number', 'Number', 0),
('file', 'File Upload', 0);
```

## 🎯 Key Features

1. **Dynamic Question Creation** - Admin can create any type of question
2. **Category-Based Access** - Users see questionnaires based on their category
3. **Flexible Response Storage** - Supports all question types
4. **Token-Based Authentication** - Secure API access
5. **Consistent Response Format** - All APIs return JsonModel
6. **Clean Architecture** - Separation of concerns
7. **Scalable Design** - Handles large datasets efficiently

## 🔍 Troubleshooting

### **Common Issues:**
1. **Token not found** - Check JWT configuration
2. **Category access denied** - Verify user's category assignment
3. **Question type not supported** - Check seeded question types
4. **File upload failed** - Verify file size and type restrictions

### **Debug Tips:**
1. Check database connections
2. Verify JWT token configuration
3. Review Entity Framework logs
4. Test API endpoints with Postman

This system provides a **robust, scalable, and maintainable** solution for dynamic questionnaire creation and response collection in healthcare applications! 🚀 
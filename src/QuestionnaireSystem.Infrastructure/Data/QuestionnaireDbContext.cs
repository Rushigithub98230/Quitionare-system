using Microsoft.EntityFrameworkCore;
using QuestionnaireSystem.Core.Models;

namespace QuestionnaireSystem.Infrastructure.Data;

public class QuestionnaireDbContext : DbContext
{
    public QuestionnaireDbContext(DbContextOptions<QuestionnaireDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CategoryQuestionnaireTemplate> CategoryQuestionnaireTemplates { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<CategoryQuestion> CategoryQuestions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<UserQuestionResponse> UserQuestionResponses { get; set; }
    public DbSet<QuestionResponse> QuestionResponses { get; set; }
    public DbSet<QuestionOptionResponse> QuestionOptionResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names
        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .ToTable("QuestionnaireTemplates");
        
        modelBuilder.Entity<CategoryQuestion>()
            .ToTable("Questions");

        // Configure relationships and constraints
        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .HasOne(q => q.Category)
            .WithOne(c => c.QuestionnaireTemplate)
            .HasForeignKey<CategoryQuestionnaireTemplate>(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // TODO: Re-enable authentication for production
        // Make CreatedBy relationship optional temporarily
        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .HasOne(q => q.CreatedByUser)
            .WithMany(u => u.CreatedQuestionnaires)
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<CategoryQuestion>()
            .HasOne(q => q.Questionnaire)
            .WithMany(qt => qt.Questions)
            .HasForeignKey(q => q.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CategoryQuestion>()
            .HasOne(q => q.QuestionType)
            .WithMany(qt => qt.Questions)
            .HasForeignKey(q => q.QuestionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionOption>()
            .HasOne(qo => qo.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(qo => qo.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserQuestionResponse>()
            .HasOne(uqr => uqr.User)
            .WithMany(u => u.Responses)
            .HasForeignKey(uqr => uqr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserQuestionResponse>()
            .HasOne(uqr => uqr.Questionnaire)
            .WithMany(qt => qt.UserResponses)
            .HasForeignKey(uqr => uqr.QuestionnaireId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionResponse>()
            .HasOne(qr => qr.Response)
            .WithMany(uqr => uqr.QuestionResponses)
            .HasForeignKey(qr => qr.ResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionResponse>()
            .HasOne(qr => qr.Question)
            .WithMany(q => q.Responses)
            .HasForeignKey(qr => qr.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionOptionResponse>()
            .HasOne(qor => qor.QuestionResponse)
            .WithMany(qr => qr.OptionResponses)
            .HasForeignKey(qor => qor.QuestionResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionOptionResponse>()
            .HasOne(qor => qor.Option)
            .WithMany(qo => qo.OptionResponses)
            .HasForeignKey(qor => qor.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.IsActive);

        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .HasIndex(qt => qt.IsActive);

        // Temporarily comment out unique constraints to allow migration
        // modelBuilder.Entity<QuestionnaireTemplate>()
        //     .HasIndex(qt => qt.CategoryId)
        //     .IsUnique(); // Ensure one template per category

        // modelBuilder.Entity<Question>()
        //     .HasIndex(q => new { q.QuestionnaireId, q.DisplayOrder })
        //     .IsUnique(); // Ensure unique question order within template

        modelBuilder.Entity<QuestionOption>()
            .HasIndex(qo => new { qo.QuestionId, qo.DisplayOrder });

        modelBuilder.Entity<UserQuestionResponse>()
            .HasIndex(uqr => uqr.UserId);

        modelBuilder.Entity<UserQuestionResponse>()
            .HasIndex(uqr => uqr.QuestionnaireId);

        // Global query filters for soft deletes
        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => c.DeletedAt == null);

        modelBuilder.Entity<CategoryQuestionnaireTemplate>()
            .HasQueryFilter(qt => qt.DeletedAt == null);

        modelBuilder.Entity<CategoryQuestion>()
            .HasQueryFilter(q => q.DeletedAt == null);

        // Seed question types
        modelBuilder.Entity<QuestionType>().HasData(
            new QuestionType { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), TypeName = "text", DisplayName = "Text Input", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), TypeName = "textarea", DisplayName = "Text Area", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), TypeName = "radio", DisplayName = "Radio Button", HasOptions = true, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), TypeName = "checkbox", DisplayName = "Checkbox", HasOptions = true, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), TypeName = "select", DisplayName = "Dropdown", HasOptions = true, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), TypeName = "multiselect", DisplayName = "Multi-Select", HasOptions = true, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), TypeName = "number", DisplayName = "Number", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), TypeName = "date", DisplayName = "Date", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), TypeName = "email", DisplayName = "Email", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), TypeName = "phone", DisplayName = "Phone", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), TypeName = "file", DisplayName = "File Upload", HasOptions = false, SupportsFileUpload = true, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), TypeName = "rating", DisplayName = "Rating Scale", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), TypeName = "slider", DisplayName = "Slider", HasOptions = false, SupportsImage = true },
            new QuestionType { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), TypeName = "yes_no", DisplayName = "Yes/No", HasOptions = true, SupportsImage = true }
        );

        // Admin user is now seeded through migrations
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Category category)
                category.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is CategoryQuestionnaireTemplate template)
                template.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is CategoryQuestion question)
                question.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is UserQuestionResponse response)
                response.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is QuestionResponse questionResponse)
                questionResponse.UpdatedAt = DateTime.UtcNow;
        }
    }
} 
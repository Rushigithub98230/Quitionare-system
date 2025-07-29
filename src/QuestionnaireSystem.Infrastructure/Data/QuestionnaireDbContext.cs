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
    public DbSet<QuestionnaireTemplate> QuestionnaireTemplates { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<PatientQuestionnaireAssignment> PatientQuestionnaireAssignments { get; set; }
    public DbSet<PatientResponse> PatientResponses { get; set; }
    public DbSet<QuestionResponse> QuestionResponses { get; set; }
    public DbSet<QuestionOptionResponse> QuestionOptionResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints
        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasOne(q => q.Category)
            .WithMany(c => c.Questionnaires)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Questionnaire)
            .WithMany(qt => qt.Questions)
            .HasForeignKey(q => q.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.QuestionType)
            .WithMany(qt => qt.Questions)
            .HasForeignKey(q => q.QuestionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionOption>()
            .HasOne(qo => qo.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(qo => qo.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientQuestionnaireAssignment>()
            .HasOne(pqa => pqa.Questionnaire)
            .WithMany(qt => qt.Assignments)
            .HasForeignKey(pqa => pqa.QuestionnaireId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PatientResponse>()
            .HasOne(pr => pr.Assignment)
            .WithMany(pqa => pqa.PatientResponses)
            .HasForeignKey(pr => pr.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientResponse>()
            .HasOne(pr => pr.Questionnaire)
            .WithMany()
            .HasForeignKey(pr => pr.QuestionnaireId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestionResponse>()
            .HasOne(qr => qr.Response)
            .WithMany(pr => pr.QuestionResponses)
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

        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasIndex(qt => qt.IsActive);

        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasIndex(qt => qt.CategoryId);

        modelBuilder.Entity<Question>()
            .HasIndex(q => new { q.QuestionnaireId, q.DisplayOrder });

        modelBuilder.Entity<QuestionOption>()
            .HasIndex(qo => new { qo.QuestionId, qo.DisplayOrder });

        modelBuilder.Entity<PatientQuestionnaireAssignment>()
            .HasIndex(pqa => pqa.PatientId);

        modelBuilder.Entity<PatientQuestionnaireAssignment>()
            .HasIndex(pqa => pqa.Status);

        // Global query filters for soft deletes
        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => c.DeletedAt == null);

        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasQueryFilter(qt => qt.DeletedAt == null);

        modelBuilder.Entity<Question>()
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
            else if (entry.Entity is QuestionnaireTemplate template)
                template.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is Question question)
                question.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is PatientResponse response)
                response.UpdatedAt = DateTime.UtcNow;
            else if (entry.Entity is QuestionResponse questionResponse)
                questionResponse.UpdatedAt = DateTime.UtcNow;
        }
    }
} 
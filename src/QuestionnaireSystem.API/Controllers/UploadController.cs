using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireSystem.Core.Models;
using System.IO;

namespace QuestionnaireSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public UploadController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<JsonModel>> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = "No file provided",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = "File size exceeds 10MB limit",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = "Invalid file type. Allowed: PDF, JPG, PNG, DOC, DOCX",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate file URL
            var fileUrl = $"/uploads/{fileName}";

            return Ok(new JsonModel
            {
                Success = true,
                Data = new
                {
                    fileUrl = fileUrl,
                    fileName = file.FileName,
                    fileSize = file.Length
                },
                Message = "File uploaded successfully",
                StatusCode = HttpStatusCodes.OK
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new JsonModel
            {
                Success = false,
                Message = $"Error uploading file: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            });
        }
    }

    [HttpPost("multiple")]
    public async Task<ActionResult<JsonModel>> UploadMultipleFiles(IFormFileCollection files)
    {
        try
        {
            if (files == null || files.Count == 0)
                return BadRequest(new JsonModel
                {
                    Success = false,
                    Message = "No files provided",
                    StatusCode = HttpStatusCodes.BadRequest
                });

            var uploadResults = new List<object>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    uploadResults.Add(new
                    {
                        fileName = file.FileName,
                        success = false,
                        message = "File size exceeds 10MB limit"
                    });
                    continue;
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    uploadResults.Add(new
                    {
                        fileName = file.FileName,
                        success = false,
                        message = "Invalid file type"
                    });
                    continue;
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate file URL
                var fileUrl = $"/uploads/{fileName}";

                uploadResults.Add(new
                {
                    fileName = file.FileName,
                    fileUrl = fileUrl,
                    fileSize = file.Length,
                    success = true
                });
            }

            return Ok(new JsonModel
            {
                Success = true,
                Data = uploadResults,
                Message = "Files uploaded successfully",
                StatusCode = HttpStatusCodes.OK
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new JsonModel
            {
                Success = false,
                Message = $"Error uploading files: {ex.Message}",
                StatusCode = HttpStatusCodes.InternalServerError
            });
        }
    }
} 
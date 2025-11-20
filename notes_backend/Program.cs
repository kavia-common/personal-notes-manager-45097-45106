using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NotesBackend.Dtos;
using NotesBackend.Models;
using NotesBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
// Configure OpenAPI/Swagger with metadata and tags
builder.Services.AddOpenApiDocument(cfg =>
{
    cfg.Title = "Personal Notes API";
    cfg.Version = "v1";
    cfg.Description = "A simple API to manage personal notes (create, list, get, update, delete).";
    cfg.PostProcess = document =>
    {
        document.Tags = new[]
        {
            new NSwag.OpenApiTag { Name = "Notes", Description = "CRUD operations for notes" }
        };
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register in-memory repository
builder.Services.AddSingleton<INoteRepository, InMemoryNoteRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Health check endpoint
app.MapGet("/", () => new { message = "Healthy" })
   .WithName("HealthCheck")
   .WithTags("Health");

// Notes endpoints
var notes = app.MapGroup("/notes").WithTags("Notes");

/// <summary>
/// Create a new note.
/// </summary>
notes.MapPost("/", async ([FromBody] CreateNoteRequest request, INoteRepository repo) =>
{
    if (request is null)
        return Results.BadRequest(new { error = "Request body is required." });

    // Validate using data annotations
    var context = new ValidationContext(request);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, context, results, true))
    {
        return Results.ValidationProblem(results.ToDictionary(
            r => r.MemberNames.FirstOrDefault() ?? "body",
            r => new[] { r.ErrorMessage ?? "Validation error" }));
    }

    var now = DateTime.UtcNow;
    var note = new Note
    {
        Id = Guid.NewGuid(),
        Title = request.Title.Trim(),
        Content = request.Content.Trim(),
        CreatedAt = now,
        UpdatedAt = now
    };

    await repo.CreateAsync(note);
    var response = NoteResponse.From(note);

    return Results.Created($"/notes/{response.Id}", response);
})
.WithName("CreateNote")
.WithSummary("Create note")
.WithDescription("Creates a new note with title and content. Returns the created note.")
.Produces<NoteResponse>(StatusCodes.Status201Created)
.ProducesValidationProblem()
.Produces(StatusCodes.Status400BadRequest);

/// <summary>
/// List all notes.
/// </summary>
notes.MapGet("/", async (INoteRepository repo) =>
{
    var items = await repo.GetAllAsync();
    return Results.Ok(items.Select(NoteResponse.From).ToArray());
})
.WithName("ListNotes")
.WithSummary("List notes")
.WithDescription("Returns all notes ordered by most recently updated first.")
.Produces<NoteResponse[]>(StatusCodes.Status200OK);

/// <summary>
/// Get a single note by ID.
/// </summary>
notes.MapGet("/{id:guid}", async (Guid id, INoteRepository repo) =>
{
    var note = await repo.GetByIdAsync(id);
    if (note is null)
        return Results.NotFound(new { error = "Note not found" });

    return Results.Ok(NoteResponse.From(note));
})
.WithName("GetNote")
.WithSummary("Get note")
.WithDescription("Returns a single note by its unique ID.")
.Produces<NoteResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

/// <summary>
/// Update an existing note by ID.
/// </summary>
notes.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateNoteRequest request, INoteRepository repo) =>
{
    if (request is null)
        return Results.BadRequest(new { error = "Request body is required." });

    // Validate using data annotations
    var context = new ValidationContext(request);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, context, results, true))
    {
        return Results.ValidationProblem(results.ToDictionary(
            r => r.MemberNames.FirstOrDefault() ?? "body",
            r => new[] { r.ErrorMessage ?? "Validation error" }));
    }

    var existing = await repo.GetByIdAsync(id);
    if (existing is null)
        return Results.NotFound(new { error = "Note not found" });

    existing.Title = request.Title.Trim();
    existing.Content = request.Content.Trim();
    existing.UpdatedAt = DateTime.UtcNow;

    var updated = await repo.UpdateAsync(existing);
    if (!updated)
        return Results.NotFound(new { error = "Note not found" });

    return Results.Ok(NoteResponse.From(existing));
})
.WithName("UpdateNote")
.WithSummary("Update note")
.WithDescription("Updates title and content of an existing note. Updates the updatedAt timestamp.")
.Produces<NoteResponse>(StatusCodes.Status200OK)
.ProducesValidationProblem()
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound);

/// <summary>
/// Delete a note by ID.
/// </summary>
notes.MapDelete("/{id:guid}", async (Guid id, INoteRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    if (!deleted)
        return Results.NotFound(new { error = "Note not found" });

    return Results.NoContent();
})
.WithName("DeleteNote")
.WithSummary("Delete note")
.WithDescription("Deletes a note by its ID.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();
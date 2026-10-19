using FileRepositories;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Dependency injection: the only place the repository implementations are chosen
builder.Services.AddSingleton<IUserRepository, UserFileRepository>();
builder.Services.AddSingleton<IPostRepository, PostFileRepository>();
builder.Services.AddSingleton<ICommentRepository, CommentFileRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
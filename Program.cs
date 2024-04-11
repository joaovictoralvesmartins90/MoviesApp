using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Entities;
using MoviesApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyOrigin().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("desenv_mysql")!);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();

//Os servi�os s� podem ser alterados antes da aplica��o ser criada, por isso a linha abaixo vem em seguida.
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello World!");

app.MapGet("/genres", () =>
{
    List<Genre> genres = new List<Genre>(
        )
    {
        new Genre{Id = 1, Name = "Drama"},
        new Genre{Id = 2, Name = "Action"}
    };

    return genres;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.MapPost("/genres", async (Genre genre, IGenresRepository repository) =>
{
    var id = await repository.Create(genre);
    return Results.Created($"/genres/{id}", genre);
});

app.Run();

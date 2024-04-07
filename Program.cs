using MoviesApp.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyOrigin().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache();

//Os serviços só podem ser alterados antes da aplicação ser criada, por isso a linha abaixo vem em seguida.
var app = builder.Build();

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

app.Run();

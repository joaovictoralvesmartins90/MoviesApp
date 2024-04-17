using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Entities;
using MoviesApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Serviços
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyOrigin().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySQL(builder.Configuration.GetConnectionString("desenv_mysql_desktop")!);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
//Serviços

//Os serviços só podem ser alterados antes da aplicação ser criada, por isso a linha abaixo vem em seguida.
var app = builder.Build();

//middleware
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseOutputCache();
//middleware

var genresEndpoints = app.MapGroup("/genres");

genresEndpoints.MapGet("/", async (IGenresRepository repository) =>
{
    var genres = await repository.GetAll();
    return Results.Ok(genres);
});

genresEndpoints.MapGet("/{id:int}", async (int id, IGenresRepository repository) =>
{
    var genre = await repository.GetById(id);

    if (genre == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genre);
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)).Tag("genres-get"));

genresEndpoints.MapPost("/", async (Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var id = await repository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.Created($"/genres/{id}", genre);
});

genresEndpoints.MapPut("/{id:int}", async (int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return Results.NotFound();
    }

    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});

genresEndpoints.MapDelete("/{id:int}", async (int id, IGenresRepository repository, IOutputCacheStore outputCacheStore) =>
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return Results.NotFound();
    }

    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});

app.Run();

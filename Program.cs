using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Entities;
using MoviesApp.Repositories;
using System.Reflection.Metadata;

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
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

genresEndpoints.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)).Tag("genres-get"));
genresEndpoints.MapGet("/{id:int}", GetGenre);
genresEndpoints.MapPost("/", CreateGenre);
genresEndpoints.MapPut("/{id:int}",UpdateGenre);
genresEndpoints.MapDelete("/{id:int}", DeleteGenre);

app.Run();

static async Task<Ok<List<Genre>>> GetGenres(IGenresRepository repository)
{
    var genres = await repository.GetAll();
    return TypedResults.Ok(genres);
}

static async Task<Results<Ok<Genre>, NotFound>> GetGenre(int id, IGenresRepository repository)
{
    var genre = await repository.GetById(id);

    if (genre == null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(genre);
}

static async Task<Results<NoContent, NotFound>> UpdateGenre(int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore)
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();
    }

    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get",default);
    return TypedResults.NoContent();
}

static async Task<Results<NoContent, NotFound>> DeleteGenre(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();
    }

    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}

static async Task<Created<Genre>> CreateGenre(Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore)
{
    var id = await repository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.Created($"/genres/{id}", genre);
}
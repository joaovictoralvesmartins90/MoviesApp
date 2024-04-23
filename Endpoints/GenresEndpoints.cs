using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MoviesApp.Entities;
using MoviesApp.Repositories;

namespace MoviesApp.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)).Tag("genres-get"));
            group.MapGet("/{id:int}", GetGenre);
            group.MapPost("/", CreateGenre);
            group.MapPut("/{id:int}", UpdateGenre);
            group.MapDelete("/{id:int}", DeleteGenre);
            return group;
        }

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
            await outputCacheStore.EvictByTagAsync("genres-get", default);
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
    }
}

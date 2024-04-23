using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MoviesApp.DTOs;
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

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository)
        {
            var genres = await repository.GetAll();
            var genresDto = genres.Select(g => new GenreDTO { Id = g.Id, Name = g.Name }).ToList();
            return TypedResults.Ok(genresDto);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetGenre(int id, IGenresRepository repository)
        {
            var genre = await repository.GetById(id);

            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            var genreDto = new GenreDTO
            {
                Id = id,
                Name = genre.Name
            };

            return TypedResults.Ok(genreDto);
        }

        static async Task<Results<NoContent, NotFound>> UpdateGenre(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            var genre = new Genre
            {
                Id = id,
                Name = createGenreDTO.Name
            };

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

        static async Task<Created<GenreDTO>> CreateGenre(CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var genre = new Genre
            {
                Name = createGenreDTO.Name,
            };

            var id = await repository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDto = new GenreDTO
            {
                Id = id,
                Name = createGenreDTO.Name
            };

            return TypedResults.Created($"/genres/{id}", genreDto);
        }
    }
}

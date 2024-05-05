using AutoMapper;
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

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository, IMapper mapper,
            int page = 1, int recordsPerPage = 10)
        {
            PaginationDTO pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage};
            var genres = await repository.GetAll(pagination);
            var genresDto = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDto);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetGenre(int id, IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetById(id);

            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            var genreDto = mapper.Map<GenreDTO>(genre);

            return TypedResults.Ok(genreDto);
        }

        static async Task<Results<NoContent, NotFound>> UpdateGenre(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

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

        static async Task<Created<GenreDTO>> CreateGenre(CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var genre = new Genre
            {
                Name = createGenreDTO.Name,
            };

            var id = await repository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDto = mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genres/{id}", genreDto);
        }
    }
}

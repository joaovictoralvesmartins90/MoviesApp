
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesApp.DTOs;
using MoviesApp.Entities;
using MoviesApp.Repositories;

namespace MoviesApp.Endpoints
{
    public static class ActorsEndpoints
    {
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActors).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)).Tag("actors-get"));
            group.MapGet("/{id:int}", GetActorById);
            group.MapPost("/", AddActor).DisableAntiforgery();
            group.MapPut("/{id:int}", UpdateActor);
            group.MapDelete("/{id:int}", DeleteActor);
            return group;
        }

        static async Task<Results<NoContent, NotFound>> DeleteActor(int id, IActorsRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> UpdateActor(int id, CreateActorDTO createActorDTO, IActorsRepository repository,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }

            var actor = mapper.Map<Actor>(createActorDTO);
            actor.Id = id;

            await repository.Update(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        private static async Task<Created<ActorDTO>> AddActor([FromForm] CreateActorDTO createActorDTO, IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var actor = mapper.Map<Actor>(createActorDTO);
            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);

            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDto);
        }

        private static async Task<Results<Ok<ActorDTO>, NotFound>> GetActorById(int id, IActorsRepository repository, IMapper mapper)
        {
            var actor = await repository.GetById(id);
            if (actor == null)
            {
                return TypedResults.NotFound();
            }
            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDto);
        }

        static async Task<Ok<List<ActorDTO>>> GetActors(IActorsRepository repository, IMapper mapper)
        {
            var actors = await repository.GetAll();
            var actorsDtos = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDtos);
        }
    }
}

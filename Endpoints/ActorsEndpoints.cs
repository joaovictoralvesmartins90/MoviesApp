
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
            group.MapGet("/", AddActor);
            group.MapGet("/{id:int}", GetActorById);
            return group;
        }

        private static async Task<Created<ActorDTO>> AddActor(CreateActorDTO createActorDTO, IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var actor = new Actor
            {
                Name = createActorDTO.Name,
                DateOfBirth = createActorDTO.DateOfBirth,
                Picture = createActorDTO.Picture,
            };
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

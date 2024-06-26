﻿
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesApp.DTOs;
using MoviesApp.Entities;
using MoviesApp.Repositories;
using MoviesApp.Services;

namespace MoviesApp.Endpoints
{
    public static class ActorsEndpoints
    {
        private readonly static string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActors).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)).Tag("actors-get"));
            group.MapGet("/{id:int}", GetActorById);
            group.MapPost("/", AddActor).DisableAntiforgery();
            group.MapPut("/{id:int}", UpdateActor).DisableAntiforgery();
            group.MapDelete("/{id:int}", DeleteActor);
            group.MapGet("/getbyname/{name}", GetActorsByName);
            return group;
        }

        private static async Task<Ok<List<ActorDTO>>> GetActorsByName(string name, IActorsRepository repository, IMapper mapper)
        {
            var actors = await repository.GetByName(name);
            var actorsDtos = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDtos);
        }

        static async Task<Results<NoContent, NotFound>> DeleteActor(int id, IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IFileStorage fileStorage)
        {
            var actor = await repository.GetById(id);

            if (actor is null)
            {
                return TypedResults.NotFound();
            }
            await fileStorage.Delete(actor.Picture, container);
            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> UpdateActor(int id, [FromForm] CreateActorDTO createActorDTO, IActorsRepository repository,
            IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage filestorage)
        {
            var actor = await repository.GetById(id);

            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorUpdate = mapper.Map<Actor>(createActorDTO);
            actorUpdate.Id = id;
            actorUpdate.Picture = actor.Picture;

            if (createActorDTO.Picture is not null)
            {
                var url = await filestorage.Edit(actorUpdate.Picture, container, createActorDTO.Picture);
                actorUpdate.Picture = url;
            }

            await repository.Update(actorUpdate);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        private static async Task<Created<ActorDTO>> AddActor([FromForm] CreateActorDTO createActorDTO, IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper, IFileStorage fileStorage)
        {
            var actor = mapper.Map<Actor>(createActorDTO);

            if (createActorDTO.Picture is not null)
            {
                var url = await fileStorage.Store(container, createActorDTO.Picture);
                actor.Picture = url;
            }

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

        static async Task<Ok<List<ActorDTO>>> GetActors(IActorsRepository repository, IMapper mapper,
            int page = 1, int recordsPerPage = 10)
        {
            PaginationDTO pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage};
            var actors = await repository.GetAll(pagination);
            var actorsDtos = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDtos);
        }
    }
}

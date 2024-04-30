using AutoMapper;
using MoviesApp.DTOs;
using MoviesApp.Entities;

namespace MoviesApp.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>().ForMember(x => x.Picture, options => options.Ignore());
        }
    }
}

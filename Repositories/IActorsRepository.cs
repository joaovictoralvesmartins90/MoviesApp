﻿using MoviesApp.DTOs;
using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetByName(string name);
    }
}

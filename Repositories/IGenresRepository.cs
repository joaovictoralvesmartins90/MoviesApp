﻿using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
    }
}

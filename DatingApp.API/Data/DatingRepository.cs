using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingApp.API.helpers;
using System;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }   

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId)
            .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user =await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userparams)
        {
            var users = _context.Users.Include(p=>p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userparams.userId);
            users = users.Where(u => u.Gender == userparams.Gender);
            if(userparams.MinAge != 18 || userparams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userparams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userparams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            }
            //if string is null or empty userparams.orderby
             if (!string.IsNullOrEmpty(userparams.OrderBy))
            {
                switch (userparams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreatedTask(users,userparams.PageNumber,userparams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() >0;
        }

    }
}
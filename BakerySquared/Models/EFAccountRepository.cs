using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BakerySquared.Models
{
    public class EFAccountRepository : IAccountRepository
    {
        private ApplicationDbContext _dbContext = new ApplicationDbContext();

        //public ApplicationUser GetUser(string userId)
        //{
        //    return (from c in _dbContext.Users where c.Id == userId select c).FirstOrDefault();
        //}

        public IEnumerable<ApplicationUser> ListUsers()
        {
            return _dbContext.Users.ToList();
        }

        //public ApplicationUser CreateUser(ApplicationUser user)
        //{
        //    _dbContext.Users.Add(user);
        //    _dbContext.SaveChanges();
        //    return user;
        //}

        //public ApplicationUser EditUser(ApplicationUser user)
        //{
            
        //}
    }
}
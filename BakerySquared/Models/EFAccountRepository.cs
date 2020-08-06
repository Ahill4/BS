using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BakerySquared.Models
{
    public class EFAccountRepository : IAccountRepository
    {
        private ApplicationDbContext db = new ApplicationDbContext();        

        public IEnumerable<ApplicationUser> ToList()
        {
            return db.Users.ToList();
        }
    }
}
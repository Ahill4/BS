using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakerySquared.Models
{
    public interface IAccountRepository
    {
        // ApplicationUser CreateUser(ApplicationUser user);

        // ApplicationUser GetUser(string userId);

        IEnumerable<ApplicationUser> ListUsers();

        //void DeleteUser(ApplicationUser user);

        //ApplicationUser EditUser(ApplicationUser user);


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakerySquared.Models
{
    public interface IAccountRepository
    {
        IEnumerable<ApplicationUser> ToList();
    }
}

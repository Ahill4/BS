using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSDB.Models;

namespace BakerySquared.Models
{
    public interface IDesksRepository
    {
        Desk CreateDesk(Desk desk);

        Desk EditDesk(string Id);

        Desk FindDesk(string Id);

        Desk DeleteDesk(string Id);

        IEnumerable<Desk> ToList();
    }
}
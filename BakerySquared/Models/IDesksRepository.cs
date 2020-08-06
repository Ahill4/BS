using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSDB.Models;

namespace BakerySquared.Models
{
    public interface IDesksRepository
    {
        bool AlreadyExists(Desk desk);

        void Create(Desk desk);

        void Edit(Desk desk);

        Desk Find(string Id);

        void Delete(string Id);

        IEnumerable<Desk> ToList();
    }
}
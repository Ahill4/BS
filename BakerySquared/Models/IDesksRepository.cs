using System;
using BakerySquared.Models;
using BSDB.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Linq;

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

        IQueryable<Desk> ToQuery();

        IQueryable<Desk> Contains(IQueryable<Desk> desks, string searchString);

        IQueryable<Desk> OrderByDescendingId(IQueryable<Desk> desks);

        IQueryable<Desk> OrderByDescendingOccupant(IQueryable<Desk> desks);

        IQueryable<Desk> OrderByAscendingId(IQueryable<Desk> desks);

        IQueryable<Desk> OrderByAscendingOccupant(IQueryable<Desk> desks);
    }
}
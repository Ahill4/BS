using System;
using BakerySquared.Models;
//using BSDB.Models;
using BakerySquared.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Linq;

namespace BakerySquared.Models
{
    public class EFDesksRepository : IDesksRepository
    {
        private BakerySquareDirectoryEntities db = new BakerySquareDirectoryEntities();

        public bool AlreadyExists(Desk desk)
        {
            Desk deskTest = db.Desks.Find(desk.Desk_Id);

            if (deskTest == null)
            {
                return false;
            }

            return true;
        }

        public void Create(Desk desk)
        {
            db.Desks.Add(desk);
            db.SaveChanges();
        }

        public void Edit(Desk desk)
        {
            db.Entry(desk).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Desk Find(string Id)
        {
            Desk desk = db.Desks.Find(Id);

            return (desk);
        }

        public void Delete(string Id)
        {
            Desk desk = db.Desks.Find(Id);
            db.Desks.Remove(desk);
            db.SaveChanges();
        }

        public IEnumerable<Desk> ToList()
        {
            return db.Desks.ToList();
        }

        public IQueryable<Desk> ToQuery()
        {
            return from d in db.Desks select d;
        }

        public IQueryable<Desk> Contains(IQueryable<Desk> desks, string searchString)
        {
            var newDesks = desks.Where(d => d.Desk_Id.Contains(searchString) || d.Occupant.Contains(searchString));

            return newDesks;
        }

        public IQueryable<Desk> OrderByDescendingId(IQueryable<Desk> desks)
        {
            var newDesks = desks.OrderByDescending(d => d.Desk_Id);

            return newDesks;
        }

        public IQueryable<Desk> OrderByDescendingOccupant(IQueryable<Desk> desks)
        {
            var newDesks = desks.OrderByDescending(d => d.Occupant);

            return newDesks;
        }

        public IQueryable<Desk> OrderByAscendingId(IQueryable<Desk> desks)
        {
            var newDesks = desks.OrderBy(d => d.Desk_Id);

            return newDesks;
        }

        public IQueryable<Desk> OrderByAscendingOccupant(IQueryable<Desk> desks)
        {
            var newDesks = desks.OrderBy(d => d.Occupant);

            return newDesks;
        }

    }
}
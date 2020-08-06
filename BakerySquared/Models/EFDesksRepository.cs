using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BSDB.Models;

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

    }
}
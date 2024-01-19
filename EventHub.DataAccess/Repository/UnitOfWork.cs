using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IEventRepository Event { get; private set; }
        public IUserEventRepository UserEvent { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Event = new EventRepository(_db);
            UserEvent = new UserEventRepository(_db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

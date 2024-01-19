using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepository;
using EventHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private ApplicationDbContext _db;
        public EventRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Event eventObj)
        {
            var existingEvent = _db.Events.Find(eventObj.Id);

            if (existingEvent != null)
            {
                // Detach the existing entity from the context
                _db.Entry(existingEvent).State = EntityState.Detached;

                // Attach the modified entity
                _db.Events.Update(eventObj);
            }
            else
            {
                // If the entity doesn't exist in the context, simply update it
                _db.Events.Update(eventObj);
            }
        }
    }
}

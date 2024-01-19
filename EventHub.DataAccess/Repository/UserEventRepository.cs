using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepository;
using EventHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository
{
    public class UserEventRepository : Repository<UserEvent>, IUserEventRepository
    {
        private ApplicationDbContext _db;
        public UserEventRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(UserEvent userEvent)
        {
            _db.UserEvent.Update(userEvent);
        }
    }
}

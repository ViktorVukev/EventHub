using EventHub.DataAccess.Repository.IRepository;
using EventHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

namespace EventHubWeb.Areas.User.Controllers
{
    [Area("User")]
    public class EventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Event> events = _unitOfWork.Event.GetAll().ToList();
            foreach (var item in events)
            {
                var category = _unitOfWork.Category.GetAll().FirstOrDefault(c => c.Id == item.CategoryId);

                if (category != null)
                {
                    item.Category.Name = category.Name;
                }
            }
            return View(events);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Event? existingEvent = _unitOfWork.Event.Get(c => c.Id == id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            /* Check if current event is booked by user */
            ViewBag.IsAlreadyBooked = false;
            IEnumerable<UserEvent> allUserEvents = _unitOfWork.UserEvent.GetAll().ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var item in allUserEvents)
            {
                if (item.UserId == userId && item.EventId == existingEvent.Id)
                {
                    ViewBag.IsAlreadyBooked = true;
                }
            }

            ViewBag.EventId = existingEvent.Id;
            ViewBag.CategoryList = CategoryList;
            ViewBag.ImageUrl = existingEvent.ImageUrl;

            return View(existingEvent);
        }

        [HttpPost]
        public IActionResult Details(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _unitOfWork.UserEvent.Add(new UserEvent
                {
                    UserId = userId,
                    EventId = id
                });
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult MyEvents()
        {
            List<Event> userEvents = new List<Event>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                IEnumerable<UserEvent> userEvent = _unitOfWork.UserEvent.GetAll().ToList();
                
                foreach (var item in userEvent)
                {
                    if (item.UserId == userId)
                    {
                        userEvents.Add(_unitOfWork.Event.Get(e => e.Id == item.EventId));
                    }
                }
            }
            return View(userEvents);
        }
    }
}

using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace EventHubWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class EventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EventController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var events = _unitOfWork.Event.GetAll().ToList();
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

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            ViewBag.CategoryList = CategoryList;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Event eventObj, IFormFile? file)
        {
            if (eventObj.Name.ToLower() == eventObj.Description.ToLower())
            {
                ModelState.AddModelError("name", "Името и описанието на събитието не могат да съвпадат.");
            }

            if (ModelState.IsValid)
            {
                /* Image upload */
                if (file != null)
                {
                    eventObj.ImageUrl = UploadeFile(file);
                }
                else
                {
                    /* Set default image */
                    eventObj.ImageUrl = @"\images\default.jpg";
                }

                _unitOfWork.Event.Add(eventObj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
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
            ViewBag.CategoryList = CategoryList;
            ViewBag.ImageUrl = existingEvent.ImageUrl;
            return View(existingEvent);
        }

        [HttpPost]
        public IActionResult Edit(Event eventObj, IFormFile? file)
        {
            if (eventObj.Name.ToLower() == eventObj.Description.ToLower())
            {
                ModelState.AddModelError("name", "Името и описанието на събитието не могат да съвпадат.");
            }
            if (ModelState.IsValid)
            {
                /* Image upload */
                if (file != null)
                {
                    eventObj.ImageUrl = UploadeFile(file);
                }
                else
                {
                    // No new file uploaded, retain the previous ImageUrl
                    Event existingEvent = _unitOfWork.Event.Get(c => c.Id == eventObj.Id);
                    if (existingEvent != null)
                    {
                        eventObj.ImageUrl = existingEvent.ImageUrl;
                    }
                }

                _unitOfWork.Event.Update(eventObj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
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
            ViewBag.CategoryList = CategoryList;
            ViewBag.ImageUrl = existingEvent.ImageUrl;

            return View(existingEvent);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult POSTDelete(int? id)
        {
            Event? eventObj = _unitOfWork.Event.Get(c => c.Id == id);
            if (eventObj == null)
            {
                return NotFound();
            }
            _unitOfWork.Event.Remove(eventObj);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public string UploadeFile(IFormFile file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string eventPath = Path.Combine(wwwRootPath, @"images\event");

            using (var fileStream = new FileStream(Path.Combine(eventPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return @"\images\event\" + fileName;
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var events = _unitOfWork.Event.GetAll().ToList();
            foreach (var item in events)
            {
                var category = _unitOfWork.Category.GetAll().FirstOrDefault(c => c.Id == item.CategoryId);

                if (category != null)
                {
                    item.Category.Name = category.Name;
                }
            }
            return Json(new { data = events });
        }

        #endregion
    }
}

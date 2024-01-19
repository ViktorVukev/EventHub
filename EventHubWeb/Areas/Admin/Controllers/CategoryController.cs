using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHubWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category cat)
        {
            if (cat.Name.ToLower() == cat.Description.ToLower())
            {
                ModelState.AddModelError("name", "Името и описанието на категорията не могат да съвпадат.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(cat);
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
            Category? existingCategory = _unitOfWork.Category.Get(c => c.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            return View(existingCategory);
        }

        [HttpPost]
        public IActionResult Edit(Category cat)
        {
            if (cat.Name.ToLower() == cat.Description.ToLower())
            {
                ModelState.AddModelError("name", "Името и описанието на категорията не могат да съвпадат.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(cat);
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
            Category? existingCategory = _unitOfWork.Category.Get(c => c.Id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            return View(existingCategory);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult POSTDelete(int? id)
        {
            Category? category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

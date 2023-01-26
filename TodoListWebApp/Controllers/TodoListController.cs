using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using TodoListWebApp.Data;
using TodoListWebApp.Interface;
using TodoListWebApp.Models;
using System.Data;

namespace TodoListWebApp.Controllers
{
    public class TodoListController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TodoListController> _logger;
        private readonly IEmailService _emailservice;
        



        public TodoListController(AppDbContext db, ILogger<TodoListController> logger, IEmailService emailservice)
        {
            _db = db;
            _logger = logger;
            _emailservice = emailservice;

     
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index Page");

            try
            {
                IEnumerable<TodoList> todoList = _db.TodoLists;
                return View(todoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "This Exception From Index Page");
                throw new Exception();
            }
        }

        //get
        public IActionResult Create()
        {
            return View();
        }

        //post
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(TodoList todo)
        public async Task<IActionResult> Create(TodoList todo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var oddEmail = _db.TodoLists.FirstOrDefault(x => x.EmailAddress == todo.EmailAddress);
                    //if (oddEmail != null)
                    //{
                    //    TempData["error"] = $"A user exist with thesame email address: {todo.EmailAddress}";
                    //    return RedirectToAction("Index");
                    //}
                    //else
                    //{
                        _db.TodoLists.Add(todo);
                        _db.SaveChanges();
                        var emailMsg = new EmailMessageContent(new string[] { todo.EmailAddress }, "NEW TODO ALERT FROM VATINTERNS", $"Dear <b> {todo.Name} </b><br> We are pleased to inform you that you have a new Todo request <br><br> Title: {todo.ListContent} <br> Dated: {todo.Time} <br> Priority: {todo.Priority} <br><br> Best Regards");
                        await _emailservice.SendEmailAsync(emailMsg);
                        TempData["success"] = "Todo List added successfully!";
                        return RedirectToAction("Index");
                    }

                //}
                return View(todo);
            }
            catch (Exception)
            {
                throw;
                //ModelState.AddModelError("", "Unable to save changes. Try Again!. if the problem persists, see your system administrator.");
            }
            
        }

        //GET
        public IActionResult Edit(string id)
        {

            string decryptId = Encryption.decrypt(id);
            if (id == null || id == "")
            {
                return NotFound();
            }

            id = decryptId;
            int newId = Convert.ToInt32(id);
            var TodoListFromDb = _db.TodoLists.Where(x => x.Id == newId).Select(x => new TodoListViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                ListContent = x.ListContent,
                Time = x.Time,
                Priority = x.Priority

            }).FirstOrDefault();
            if (TodoListFromDb == null)
            {
                return NotFound();
            }

            return View("Edit", TodoListFromDb);
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TodoListViewModel todo, string id)
        {
            try
            {

                string decryptId = Encryption.decrypt(id);
                if (id == null || id == "")
                {
                    return NotFound();
                }

                id = decryptId;
                int newId = Convert.ToInt32(id);

                var getId = _db.TodoLists.Where(x => x.Id == newId).FirstOrDefault();
                getId.ListContent = todo.ListContent;
                getId.Time = todo.Time;
                getId.Priority = todo.Priority;
                _db.SaveChanges();
                var emailMsg = new EmailMessageContent(new string[] { todo.EmailAddress }, "UPDATE TODO ALERT FROM VATINTERNS", $"Dear <b> {todo.Name} </b><br> This is to inform you that your Todo request have been Updated <br><br> Title: {todo.ListContent} <br> Dated: {todo.Time} <br> Priority: {todo.Priority} <br><br> Best Regards");
                _emailservice.SendEmailAsync(emailMsg);
                TempData["success"] = "Todo List updated successfully!";
                return RedirectToAction("Index");

            }
            catch
            {
                ModelState.AddModelError("", "Unable to save changes. Try Again!. if the problem persists, see your system administrator.");
            }
            return View(todo);
        }

        //GET
        public IActionResult Delete(string id, bool? saveChangesError = false)
        {
            string decryptId = Encryption.decrypt(id);
            if (id == null || id == "")
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again!. if the error persists, see your system administrator";
            }
            id = decryptId;
            int newId = Convert.ToInt32(id);
            var TodoListFromDb = _db.TodoLists.Where(x => x.Id == newId).Select(x => new TodoListViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                ListContent = x.ListContent,
                Time = x.Time,
                Priority = x.Priority

            }).FirstOrDefault();
            //var TodoListFromDb = _db.TodoLists.Find(id);
            if (TodoListFromDb == null)
            {
                return NotFound();
            }
            return View("Delete", TodoListFromDb);
        }

        //post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string id)
        {
            try
            {
                string decryptId = Encryption.decrypt(id);
                id = decryptId;
                int newId = Convert.ToInt32(id);
                //var todo = _db.TodoLists.Find(id);
                var todo = _db.TodoLists.Where(x => x.Id == newId).FirstOrDefault();
                if (todo == null)
                {
                    return NotFound();
                }

                _db.TodoLists.Remove(todo);
                _db.SaveChanges();
                TempData["success"] = "Todo list deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

        }

    }
  
}

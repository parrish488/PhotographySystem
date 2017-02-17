using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotgraphyMVC.Models;

namespace PhotgraphyMVC.Controllers
{
    [Authorize]
    public class TodoListsController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: TodoLists
        public ActionResult Index(bool? incompleteOnly)
        {
            if (incompleteOnly != null && incompleteOnly.Value)
            {
                ViewBag.Title = "Incomplete Todo Items";
                return View(db.TodoList.Where(t => t.Username == User.Identity.Name).Where(t => t.IsCompleted == false).ToList());
            }
            else
            {
                ViewBag.Title = "All Todo Items";
                return View(db.TodoList.Where(t => t.Username == User.Identity.Name).ToList());
            }
        }

        // GET: TodoLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoList todoList = db.TodoList.Find(id);
            if (todoList == null)
            {
                return HttpNotFound();
            }
            return View(todoList);
        }

        // GET: TodoLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TodoLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TodoListID,ItemDescription,DueDate,IsCompleted,Username")] TodoList todoList)
        {
            if (ModelState.IsValid)
            {
                todoList.Username = User.Identity.Name;

                db.TodoList.Add(todoList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(todoList);
        }

        // GET: TodoLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoList todoList = db.TodoList.Find(id);
            if (todoList == null)
            {
                return HttpNotFound();
            }
            return View(todoList);
        }

        // POST: TodoLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TodoListID,ItemDescription,DueDate,IsCompleted,Username")] TodoList todoList)
        {
            if (ModelState.IsValid)
            {
                todoList.Username = User.Identity.Name;

                db.Entry(todoList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(todoList);
        }

        // GET: TodoLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoList todoList = db.TodoList.Find(id);
            if (todoList == null)
            {
                return HttpNotFound();
            }
            return View(todoList);
        }

        // POST: TodoLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TodoList todoList = db.TodoList.Find(id);
            db.TodoList.Remove(todoList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void CreateFromTemplate(string client, int templateId)
        {
            TodoTemplate template = db.TodoTemplates.Find(templateId);

            TodoList todoList = new TodoList();
            todoList.ItemDescription = client + " - " + template.TemplateDescription;

            switch (template.RangeValue)
            {
                case "Days":
                    todoList.DueDate = DateTime.Now.AddDays(template.RangeUnits);
                    break;
                case "Weeks":
                    todoList.DueDate = DateTime.Now.AddDays(template.RangeUnits * 7);
                    break;
                case "Months":
                    todoList.DueDate = DateTime.Now.AddMonths(template.RangeUnits);
                    break;
            }

            todoList.IsCompleted = false;
            todoList.Username = User.Identity.Name;

            db.TodoList.Add(todoList);
            db.SaveChanges();
        }

        public ActionResult CompleteItem(int? id, bool fromDashboard = true)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TodoList todoList = db.TodoList.Find(id);

            if (todoList == null)
            {
                return HttpNotFound();
            }
            else
            {
                todoList.IsCompleted = true;
                db.SaveChanges();
            }

            if (fromDashboard)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "TodoLists");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

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
    public class TodoTemplatesController : Controller
    {
        private PhotographerContext db = new PhotographerContext();

        // GET: TodoTemplates
        public ActionResult Index()
        {
            return View(db.TodoTemplates.Where(t => t.Username == User.Identity.Name).ToList());
        }

        // GET: TodoTemplates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoTemplate todoTemplate = db.TodoTemplates.Find(id);
            if (todoTemplate == null)
            {
                return HttpNotFound();
            }
            return View(todoTemplate);
        }

        // GET: TodoTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TodoTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TodoTemplateID,TemplateDescription,RangeUnits,RangeValue,Username")] TodoTemplate todoTemplate)
        {
            if (ModelState.IsValid)
            {
                todoTemplate.Username = User.Identity.Name;
                db.TodoTemplates.Add(todoTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(todoTemplate);
        }

        // GET: TodoTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoTemplate todoTemplate = db.TodoTemplates.Find(id);
            if (todoTemplate == null)
            {
                return HttpNotFound();
            }
            return View(todoTemplate);
        }

        // POST: TodoTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TodoTemplateID,TemplateDescription,RangeUnits,RangeValue,Username")] TodoTemplate todoTemplate)
        {
            if (ModelState.IsValid)
            {
                todoTemplate.Username = User.Identity.Name;
                db.Entry(todoTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(todoTemplate);
        }

        // GET: TodoTemplates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoTemplate todoTemplate = db.TodoTemplates.Find(id);
            if (todoTemplate == null)
            {
                return HttpNotFound();
            }
            return View(todoTemplate);
        }

        // POST: TodoTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TodoTemplate todoTemplate = db.TodoTemplates.Find(id);
            db.TodoTemplates.Remove(todoTemplate);
            db.SaveChanges();
            return RedirectToAction("Index");
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

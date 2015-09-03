using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTODO.Controllers
{
  public class HomeController : Controller
  {

    public ActionResult Index()
    {
      var list = HttpContext.Session["list"] as List<MvcTODO.Models.TODOList>;

      if (list == null)
      {
          list = new List<Models.TODOList>();
      }

      return View(list);
    }

    [HttpGet]
    public ActionResult AddTODO()
    {
      ViewBag.Message = "Add TODO List.";
      
      MvcTODO.Models.TODOList todo = new Models.TODOList();

      return View(todo);
    }

    public ActionResult AddTODO(MvcTODO.Models.TODOList todo, IEnumerable<string> UserValues)
    {
      todo.ListItems.AddRange(UserValues.ToList());

      var list = HttpContext.Session["list"] as List<MvcTODO.Models.TODOList>;

      if (list == null)
      {
        list = new List<Models.TODOList>();
      }

      list.Add(todo);
      HttpContext.Session["list"] = list;
      return Redirect("/");
    }

    [HttpGet]
    public ActionResult DeleteTODO()
    {
      var list = HttpContext.Session["list"] as List<MvcTODO.Models.TODOList>;

      if (list == null)
      {
        list = new List<Models.TODOList>();
      }

      ViewBag.List = list;

      return View(new MvcTODO.Models.SelectedItems() { ListNames = list });
    }

    [HttpPost]
    public ActionResult DeleteTODO(List<int> selectedIds)
    {
      var list = HttpContext.Session["list"] as List<MvcTODO.Models.TODOList>;
      if (selectedIds != null)
      { 
        foreach (int index in selectedIds)
        {
          list.RemoveAt(index);
        }
      }

      HttpContext.Session["list"] = list;

      return Redirect("/");
    }

    public ActionResult EditTODO()
    {
      ViewBag.Message = "Edit TODO List.";

      return View();
    }
  }
}
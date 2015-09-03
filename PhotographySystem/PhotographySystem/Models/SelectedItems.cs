using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTODO.Models
{
  public class SelectedItems
  {
    public List<int> SelectedIndexes { get; set; }

    public List<TODOList> ListNames { get; set; }

    public SelectedItems()
    {
      //SelectedIndexes = new List<int>();
      //ListNames = new List<TODOList>();
    }
  }
}
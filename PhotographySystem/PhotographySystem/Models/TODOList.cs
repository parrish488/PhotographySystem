using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTODO.Models
{
  public class TODOList
  {
    public string Name { get; set; }

    public List<string> ListItems { get; set; }

    public TODOList()
    {
      ListItems = new List<string>();
    }
  }
}
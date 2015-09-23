using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Milage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CopyrightLabel.Text = "Copyright " + DateTime.Now.Year + " Alissa Paige Photography, LLC";
    }
}
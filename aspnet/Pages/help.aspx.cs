using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Library;

namespace portmgr.Pages
{
    public partial class help : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.RequestType == "POST")
            {
                if (Context.Request.Form["jvpost"] == "1")
                {
                    String email = Request.Form["email"];
                    String name = Request.Form["name"];
                    String msg = Request.Form["question"];
                    Context.Response.Redirect("supportquestsent.aspx");
                }
                else
                    Context.Response.Redirect("help.aspx");
            }
        }
    }
}
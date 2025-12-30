using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
// using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        public void Page_Load(object sender, EventArgs e)
        {

            if (password.Text == "admin" && name.Text == "admin@gmail.com")
            {
                Response.Redirect("AdminProfile.aspx");
                Server.Transfer("AdminProfile.aspx");
            }

        }
    }
}
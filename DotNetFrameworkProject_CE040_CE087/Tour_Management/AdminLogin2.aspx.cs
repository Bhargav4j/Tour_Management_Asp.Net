using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Cloud-ready: Retrieve admin credentials from configuration (environment variables)
            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            string adminPassword = System.Configuration.ConfigurationManager.AppSettings["AdminPassword"];

            if (!string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(name.Text))
            {
                if (password.Text == adminPassword && name.Text == adminEmail)
                {
                    Session["AdminEmail"] = name.Text;
                    Response.Redirect("AdminProfile.aspx");
                }
                else
                {
                    Response.Write("Invalid admin credentials");
                }
            }
        }
    }
}
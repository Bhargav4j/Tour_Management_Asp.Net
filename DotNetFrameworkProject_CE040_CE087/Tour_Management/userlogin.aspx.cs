using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class userlogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

  
            protected void Btn_Submit(object sender, EventArgs e)
            {
                // Cloud-ready: Using parameterized query to prevent SQL injection and proper using statement for resource disposal
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString))
                {
                    conn.Open();
                    // Fixed SQL injection vulnerability with parameterized query
                    string checkPasswordQuery = "select password from Userinfo where password=@password and email=@email";

                    using (SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn))
                    {
                        passComm.Parameters.AddWithValue("@password", txtPassword.Text);
                        passComm.Parameters.AddWithValue("@email", txtEmail.Text);

                        string password = passComm.ExecuteScalar()?.ToString() ?? "";

                        if (password == txtPassword.Text)
                        {
                            Session["UserEmail"] = txtEmail.Text;
                            Response.Redirect("MainProfilePage.aspx");
                        }
                        else
                        {
                            Response.Write("Password is not correct");
                        }
                    }
                }
            }

        protected void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx");
            Server.Transfer("SignUpForm.aspx");
        }
    }
   
}
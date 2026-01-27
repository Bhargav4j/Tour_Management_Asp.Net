using System;
using System.Collections.Generic;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Npgsql;
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
            
               

                NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
                conn.Open();
                string checkPasswordQuery = "SELECT password FROM userinfo WHERE password=@password AND email = @email";
                NpgsqlCommand passComm = new NpgsqlCommand(checkPasswordQuery, conn);
                passComm.Parameters.AddWithValue("@password", txtPassword.Text);
                passComm.Parameters.AddWithValue("@email", txtEmail.Text);
            string password = passComm.ExecuteScalar()?.ToString() ?? "";


              

                if (password == txtPassword.Text)
                {
                    //Session["New"] = txtEmail.Text;
                Response.Write("Password is correct");
                
                Response.Redirect("MainProfilePage.aspx");
                    Server.Transfer(  "MainProfilePage.aspx");
                }


            
            else
                {
                    Response.Write("Password is not correct");
                
            }




            }

        protected void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx");
            Server.Transfer("SignUpForm.aspx");
        }
    }
   
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
// using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class userlogin : System.Web.UI.Page
    {
        public void Page_Load(object sender, EventArgs e)
        {

        }

  
            public void Btn_Submit(object sender, EventArgs e)
            { 
            
               

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
                conn.Open();
                string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
                SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
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

        public void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx");
            Server.Transfer("SignUpForm.aspx");
        }
    }
   
}
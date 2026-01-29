using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Tour_Management
{
    public partial class TourCrud : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                refreshdata();
            }
        }
        public void refreshdata()
        {
            // Cloud-ready: Using proper disposal pattern with using statement
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString))
            {
                conn.Open();
                string selectQuery = "select * from Tour";

                using (SqlCommand com = new SqlCommand(selectQuery, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(com))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        // GridView1.DataSource = dt;
                        // GridView1.DataBind();
                    }
                }
            }
        }

       
    }
}
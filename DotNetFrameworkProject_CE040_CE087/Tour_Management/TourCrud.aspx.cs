using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Npgsql;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Tour_Management
{
    public partial class TourCrud : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                refreshdata();
            }
        }
        public void refreshdata()
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
            conn.Open();
            string insertQuery = "SELECT * FROM tour";
            NpgsqlCommand com = new NpgsqlCommand(insertQuery, conn);
          // GridView1.DataSource = insertQuery;
           // GridView1.DataBind();


            // NpgsqlConnection con = new NpgsqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database.mdf;Integrated Security=True;User Instance=True");
        //    NpgsqlCommand cmd = new NpgsqlCommand("select * from tbl_data", con);
         //   SqlDataAdapter sda = new SqlDataAdapter(cmd);
           // DataTable dt = new DataTable();
            //sda.Fill(dt);
           // GridView1.DataSource = dt;
            //GridView1.DataBind();


        }

       
    }
}
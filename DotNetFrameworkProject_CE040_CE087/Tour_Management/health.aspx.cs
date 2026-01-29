using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace Tour_Management
{
    public partial class health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json";

            try
            {
                // Check database connectivity
                string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                    }
                }

                Response.StatusCode = 200;
                Response.Write("{\"status\":\"healthy\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 503;
                Response.Write("{\"status\":\"unhealthy\",\"error\":\"" + ex.Message.Replace("\"", "\\\"") + "\",\"timestamp\":\"" + DateTime.UtcNow.ToString("o") + "\"}");
            }

            Response.End();
        }
    }
}

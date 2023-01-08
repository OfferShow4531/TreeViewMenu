using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Security.Cryptography.X509Certificates;
using TreeViewMenu.Models;
using System.Threading.Tasks;

namespace TreeViewMenu.Views.Shared
{
    public class HomesController : Controller
    {
        private SqlConnection conn;
        private SqlDataAdapter da;
        private DataTable dt;
        public Menues menues = new Menues();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        protected void DeletePost()
        {
            try
            {

                String id = Request["Id"];
                string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog = TreeviewMVC; User ID = Judgment_Developer; Password = 1963";
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql_query = "DELETE FROM Menu WHERE id = @id";
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql_query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        
        public void onPost()
        {
            menues.MenuNumber = Int32.Parse(Request.Form["menunumber"]);
            menues.ParentNumber = Int32.Parse(Request.Form["parentnumber"]);
            menues.MenuName = Request.Form["menuname"];
            menues.Uri = Request.Form["uri"];
            menues.Icon = Request.Form["icon"];
            if (menues.MenuNumber.ToString().Length == 0 || menues.ParentNumber.ToString().Length == 0 || menues.MenuName.Length == 0)
            {
                ErrorMessage = "Some fields aren't required!";
                return;
            }

            try
            {
                string sql = @"SELECT MenuNumber, ParentNumber, MenuName, Uri, Icon FROM Menu";
                conn = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; 
	                                Initial Catalog = TreeviewMVC; 
	                                User ID = Judgment_Developer; Password = 1963");
                conn.Open();
                string sql_query = "INSERT INTO Menu" +
                                "(MenuNumber, ParentNumber, MenuName, Uri, Icon) VALUES " +
                                "(@menunumber, @parentnumber, @menuname, @uri, @icon);";
                using (SqlCommand command = new SqlCommand(sql_query, conn))
                {
                    command.Parameters.AddWithValue("@menunumber", menues.MenuNumber);
                    command.Parameters.AddWithValue("@parentnumber", menues.ParentNumber);
                    command.Parameters.AddWithValue("@menuname", menues.MenuName);
                    command.Parameters.AddWithValue("@uri", menues.Uri);
                    command.Parameters.AddWithValue("@icon", menues.Icon);

                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex) { return; }

            menues.MenuNumber = Int32.Parse("");
            menues.ParentNumber = Int32.Parse("");
            menues.MenuName = "";
            menues.Uri = "";
            menues.Icon = "";
            SuccessMessage = "Successfully added!";
            Response.Redirect("/Homes/Index");
        }



        private string PopulateMenuDataTable()
        {
            string DOM = "";

            string sql = @"SELECT MenuNumber, ParentNumber, MenuName, Uri, Icon FROM Menu";
            conn = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; 
	                                Initial Catalog = TreeviewMVC; 
	                                User ID = Judgment_Developer; Password = 1963");
            conn.Open();

            da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.CommandTimeout = 10000;

            dt = new DataTable();
            da.Fill(dt);

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Dispose();

            DOM = GetDOMTreeView(dt);

            return DOM;
        }

        private string GetDOMTreeView(DataTable dt)
        {
            string DOMTreeView = "";

            DOMTreeView += CreateTreeViewOuterParent(dt);
            DOMTreeView += CreateTreeViewMenu(dt, "0");

            DOMTreeView += "</ul>";

            return DOMTreeView;
        }

        private string CreateTreeViewOuterParent(DataTable dt)
        {
            string DOMDataList = "";

            DataRow[] drs = dt.Select("MenuNumber = 0");

            foreach (DataRow row in drs)
            {
                //row[2], 2 is column number start with 0, which is the MenuName
                DOMDataList = "<ul><li class='header'>" + row[2].ToString() + "</li>";
            }

            return DOMDataList;
        }

        private string CreateTreeViewMenu(DataTable dt, string ParentNumber)
        {
            string DOMDataList = "";

            string menuNumber = "";
            string menuName = "";
            string uri = "";
            string icon = "";

            DataRow[] drs = dt.Select("ParentNumber = " + ParentNumber);

            foreach (DataRow row in drs)
            {
                menuNumber = row[0].ToString();
                menuName = row[2].ToString();
                uri = row[3].ToString();
                icon = row[4].ToString();

                DOMDataList += "<li class='treeview'>";
                DOMDataList += "<a href='" + uri + "'><i class='" + icon + "'></i><span>  "
                                + menuName + "</span></a>";

                DataRow[] drschild = dt.Select("ParentNumber = " + menuNumber);
                if (drschild.Count() != 0)
                {
                    DOMDataList += "<ul class='treeview-menu'>";
                    DOMDataList += CreateTreeViewMenu(dt, menuNumber).Replace
                                   ("<li class='treeview'>", "<li>");
                    DOMDataList += "</ul></li>";
                }
                else
                {
                    DOMDataList += "</li>";
                }
            }
            return DOMDataList;
        }


        public ActionResult Index()
        {
            ViewBag.DOM_TreeViewMenu = PopulateMenuDataTable();
            DeletePost();
            return View();
        }
        public ActionResult CreateTable()
        {
            onPost();
            return View();
        }

        
    }
}

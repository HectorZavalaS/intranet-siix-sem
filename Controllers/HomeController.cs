using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using intranet_siix_sem.Clases;

namespace intranet_siix_sem.Controllers
{
    public class HomeController : Controller
    {
        private MySqlConnector.MySqlConnection con;
        private MySqlConnector.MySqlDataReader dr;
        private MySqlConnector.MySqlCommand com;
        siixsem_connection connection = new siixsem_connection();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login(siixsem_account data)
        {
            if (Session["id"] == null)
            {
                return View();
            }
            if (Session["name"] == null)
            {
                return RedirectToAction("index", "Home");
            }

            else
            {
                con.ConnectionString = connection.conection_bd();
                try
                {
                    MD5 md5 = MD5CryptoServiceProvider.Create();
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] stream = null;
                    StringBuilder sb = new StringBuilder();
                    stream = md5.ComputeHash(encoding.GetBytes(data.password));
                    for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "select * from IOUSR where UserName='" + data.password + "' AND UserPass='" + sb.ToString() + "'";
                    //com.CommandText = "select * from IOSR";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        Session["id"] = dr["UsrCode"].ToString();
                        Session["username"] = dr["UserName"].ToString();
                        Session["uservend"] = dr["UsrVend"].ToString();
                        Session["accesslevel"] = dr["AccCode"].ToString();
                        Session["email"] = dr["Email"].ToString();
                        Session["sapalm"] = dr["Almacen"].ToString();
                        con.Close();
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        con.Close();// return ();
                        ViewBag.error = "Error en Nick o Password";
                        return RedirectToAction("Login", "Account");
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("Login", "Account");


                }
            }

        }
        [HttpPost]

        public ActionResult Verify(siixsem_account acc)
        {
            //connectionString();
            con.ConnectionString = connection.conection_bd();
            try
            {
                MD5 md5 = MD5CryptoServiceProvider.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = md5.ComputeHash(encoding.GetBytes(acc.password));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from IOUSR where UserName='" + acc.username + "' AND UserPass='" + sb.ToString() + "'";
                //com.CommandText = "select * from IOSR";
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    Session["id"] = dr["UsrCode"].ToString();
                    Session["username"] = dr["UserName"].ToString();
                    Session["uservend"] = dr["UsrVend"].ToString();
                    Session["accesslevel"] = dr["AccCode"].ToString();
                    Session["email"] = dr["Email"].ToString();
                    Session["pass"] = dr["Pass"].ToString();
                    Session["sapalm"] = dr["UsrSap"].ToString();
                    con.Close();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    con.Close();// return ();
                    ViewBag.error = "Error en Nick o Password";
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Login", "Account");
        }
        public JsonResult Verificar(siixsem_account datos)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            StringBuilder sb = new StringBuilder();
            if (datos.username != null && datos.password != null)
            {
                byte[] stream = md5.ComputeHash(encoding.GetBytes(datos.password));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                siisem_it data;
                List<siisem_it> list = new List<siisem_it>();
                string sql = "";

                sql = "SELECT * From siixsem_users where username='"+datos.username+"' and password='" + sb.ToString()/*datos.password*/ + "'";

                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                 com = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    data = new siisem_it();
                    data.id = dr["id"].ToString();
                    data.name = dr["name"].ToString();
                    data.lastname = dr["lastname"].ToString(); 
                    data.username = dr["username"].ToString();
                    data.iddep = dr["id_department"].ToString();
                    data.characteristic1 = dr["type_user"].ToString();
                    Session["id"] = dr["id"].ToString();
                    Session["name"] = dr["name"].ToString();
                    Session["lastname"] = dr["lastname"].ToString();
                    Session["username"] = dr["username"].ToString();
                    Session["password"] = dr["password"].ToString();
                    Session["id_department"] = dr["id_department"].ToString();
                    Session["type_user"] = dr["type_user"].ToString();
                    con.Close();
                    return Json(JsonRequestBehavior.AllowGet);
                }
                else
                {
                    con.Close();
                    ViewBag.error = "Error en Nick o Password";
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json("error", JsonRequestBehavior.AllowGet);
        }

        public ActionResult exit()
        {
            System.Web.HttpContext.Current.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult About()
        {
            var name= Session["name"];
            var lastname=Session["lastname"];
            ViewBag.Message = name+ " " + lastname;


            return View();
        }
        

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
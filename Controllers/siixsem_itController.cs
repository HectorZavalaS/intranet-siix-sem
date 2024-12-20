using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using intranet_siix_sem.Clases;
//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace intranet_siix_sem.Controllers
{
    public class siixsem_itController : Controller
    {
        //------------------------------------------- Variables para conexión de BD -------------------------------------------
        /*MySqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        Conexion conexion = new Conexion();*/  // Variable que hace referencia a la clase Conexión para la base de datos 

        private MySqlConnector.MySqlConnection con;
        private MySqlConnector.MySqlDataReader dr;
        private MySqlConnector.MySqlCommand com;
        siixsem_connection connection = new siixsem_connection();
        private MySqlConnector.MySqlConnection con2;
        private MySqlConnector.MySqlDataReader dr2;
        private MySqlConnector.MySqlCommand com2;
        // GET: siixsem_it
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult Facturas()
        {
            return View();
        }
        public ActionResult Altas()
        {
            return View();
        }
        public JsonResult newline(siisem_it conta)
        {
            int numero = conta.Contador + 0;
            string resultado = "<tr><td><select name=equip" + numero + " id=equip" + numero + " class='ctn form-control selectpicker' data-show-subtext=true data-live-search=true required>";
            string a, b, c, d;
            List<siisem_it> articulos = new List<siisem_it>();
            articulos = Get_EquipD();
            foreach (siisem_it p in articulos)
            {
                a = p.id;
                b = p.id_eq;
                c = p.e_name;
                d = p.code_description;
                resultado += "<option value=" + a + ">" + d + " " + c + "-" + b + "</option>";
            }
            resultado += "</select></td>";
            con.Close();
            return Json(resultado);
        }
        public JsonResult newline2(siisem_it conta)
        {
            int numero = conta.Contador - 1;
            string resultado = "<tr><td><center><select name='id_description_equipment_" + numero + "' id='id_description_equipment_" + numero + "' class='form-control'></select></center></td><td><center> <input class='form-control' id='series_" + numero + "' name='series' /></center></td><td><center><select id='brad_" + numero + "' class='form-control'></select ></center></td><td><center><select id='model_" + numero + "' class='form-control'></center></td></tr>";

            return Json(resultado);
        }
        public ActionResult PDF()
        {
            return View();
        }
        public ActionResult MasterList()
        {
            return View();
        }
        public JsonResult GetMaster_List(siisem_it data)
        {
            var result = All_masterlist(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> All_masterlist(siisem_it data)
        {
            string sql = "";
            string stat = "";
            //siisem_it data;
            List<siisem_it> list = new List<siisem_it>();

            if (data.e_status == null) stat = "";
            else stat = data.e_status.ToString();

            if (data.id_code_e == null && data.id_department == null && data.e_status == null){
                
                sql = "CALL `siixsem_it_administration`.`EQUIPOSv2`(0,0,'"+stat+"'); ";
            }
            else
            {
                sql = "CALL `siixsem_it_administration`.`EQUIPOSv2`("+data.id_code_e.ToString()+"," + data.id_department.ToString() + ",'"+stat+"'); ";
            }
            
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);

            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.e_name = dr["e_name"].ToString();
                data.series = dr["series"].ToString();
                data.model_name = dr["name_model"].ToString();
                data.name_dep = dr["name_dep"].ToString();
                //data.warranty = dr["warranty"].ToString();
                data.comments = dr["comments"].ToString();
                data.ubication = dr["ubication"].ToString();
                data.e_status = dr["e_status"].ToString();
                data.brand = dr["name_trademark"].ToString();
                data.cod_description = dr["cod_description"].ToString();
                //data.name_procesador = dr["name_computer_processing_unit"].ToString();
                //data.characteristic1 = dr["disk_capacity"].ToString();
                //data.characteristic2 = dr["disc_type"].ToString();
                //data.characteristic3 = dr["ram_memoria"].ToString();
                //data.name_file = Carta_Responsiva_Vigente(data.id);
                data.activo_fijo = dr["activo_fijo"].ToString();

                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult add_equipment_it(siisem_it data)
        {
            var result = Add_Equipment(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Add_Equipment(siisem_it data)
        {
            string response = get_duplicate(data);
            if (response != "0")
            {
                return "Ya se encuentra registrado";
            }
            else {
                ; string sem = "";
                string nombre_equipo = "";
                string tipo_dispositivo = get_type_d(data.id_code_e);
                string sql = "";
                DateTime fecha_actual = DateTime.Now;
                if (data.iddetails == null)
                {
                    data.iddetails = "0";
                }
                if (data.warranty == null)
                {
                    data.warranty = "0000-00-00";
                }
                if (data.sem != null) { sem = data.sem; } else { sem = get_lastid_eq(data.brand); }
                if (tipo_dispositivo == "LP" || tipo_dispositivo == "WS") { nombre_equipo = tipo_dispositivo + "SEM"; } else if (data.model_name == "18") { nombre_equipo = "HW"; } else { nombre_equipo = tipo_dispositivo; }
                string id_details_equipment = get_id_details_equipment(data);
                string id_id_trademark_model = get_id_trademark_model(data);
                string id_cargador = get_id_cargador(data);
                if (data.id_code_e=="2") {
                    sql = "INSERT INTO `siixsem_it_administration`.`siixsem_cargadores` (`serial`, `id_trademark`, `Estatus`, `id_sem`) VALUES ('"+ data.series + "','"+ id_id_trademark_model + "','Asignado','"+sem+"');\r\n";
                } else {
                     sql = "INSERT INTO siixsem_equipment(id_eq,e_name,series,e_status,warranty,comments,ubication,id_trademark_model,id_department,id_details_equipment,id_code_e,bitlocker,activo_fijo,id_cargador) " +
                            " VALUES ('" + sem + "','" + nombre_equipo + "','" + data.series + "','Disponible','" + data.warranty + "','" + data.comments + "','" + data.ubication + "','" + id_id_trademark_model + "','" + data.iddep + "','" + id_details_equipment + "','" + data.id_code_e +"','"+ data.bitlocker +"','"+data.activo_fijo +"','"+id_cargador+"');";
                }
                
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "OK";
            }

        }
        public string get_duplicate(siisem_it datos)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_equipment where id_code_e='" + datos.id_code_e + "' and series='" + datos.series + "'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_eq = dr["id_eq"].ToString();
                con.Close();
                return data.id_eq;
            }
            return "0";
        }
        public string get_id_cargador(siisem_it datos)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_cargadores where id_sem='"+datos.sem+"'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                con.Close();
                return data.id;
            }
            return "0";
        }
        public string get_lastid_eq(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";
            switch (id_code_e)
            {
                case "1":
                    sql = "select id_eq +1 as id_eq from siixsem_equipment where ID_CODE_E = 1 or ID_CODE_E = 3 order by id desc limit 1 ";
                    break;
                case "2":
                    sql = "select id_eq +1 as id_eq from siixsem_equipment where ID_CODE_E = 2 order by id_eq desc limit 1;";
                    break;
                case "3":
                    sql = "select id_eq +1 as id_eq from siixsem_equipment where ID_CODE_E = 1 or ID_CODE_E = 3 order by id desc limit 1 ";
                    break;
                default:
                    sql = "select id_eq  as id_eq from siixsem_equipment where ID_CODE_E = 1 or ID_CODE_E = 3 order by id desc limit 1 ";
                    break;
            }
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_eq = dr["id_eq"].ToString();
                con.Close();
                return data.id_eq;
            }
            return "";
        }
        public string get_type_d(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_code_equipment where idcode_e='" + id_code_e + "'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.code_name = dr["code_name"].ToString();
                con.Close();
                return data.code_name;
            }
            return "";
        }
        public string get_id_details_equipment(siisem_it datos)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_details_equipment where id_processor='" + datos.id_procesador + "' and id_disc_type='" + datos.id_type_disk + "' and id_ram_memory='" + datos.id_ram_memory + "' and id_disk_capacity='" + datos.disk_capacity + "'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                con.Close();
                if (datos.id_code_e == "1" || datos.id_code_e == "4")
                {
                    return data.id;
                }
                else { return "1"; }

            }
            return "1";
        }
        public string get_id_trademark_model(siisem_it datos)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_trademark_model where id_trademark='" + datos.brand + "' and id_model='" + datos.model_name + "' and id_code_equipment='" + datos.id_code_e + "';";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                con.Close();
                return data.id;
            }
            return "";
        }
        public JsonResult Get_typeEqui(siisem_it datos) {
            var result = Get_TypeEquipment(datos);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string Get_TypeEquipment(siisem_it dato)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_code_equipment where idcode_e=" + dato.id;

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.code_name = dr["code_name"].ToString();
                con.Close();
                return data.code_name;
            }
            return "";
        }
        public JsonResult Get_Devices()
        {
            var result = get_device();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_device()
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * From siixsem_code_equipment where cod_description!='Cargador'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_code_e = dr["idcode_e"].ToString();
                data.code_name = dr["code_name"].ToString();
                data.code_description = dr["cod_description"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        /*-----------------------------JSON PARA OBTENER ID DE LOS DISPOSITIVOS QUE TIENEN CARGADOR----------------------------------*/
        public JsonResult Get_Device_Car(siisem_it dato)
        {
            var result = Result_device(dato);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /*-----------------------------LISTA PARA OBTENER INFORMACION DE LOS DISPOSITIVOS QUE TIENEN CARGADOR----------------------------------*/
        public List<siisem_it> Result_device(siisem_it dato)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * From siixsem_code_equipment where idcode_e=" + dato.id_code_e + " LIMIT 1";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.code_name = dr["code_name"].ToString();
                data.id = get_id_Carg(data.code_name);

                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public string get_id_Carg(string code_name)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "select * from siixsem_code_equipment where code_name ='" + code_name + "' and cod_description='Cargador'; ";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.id = dr2["idcode_e"].ToString();
                con2.Close();
                return data.id;
            }
            return "";
        }
        public JsonResult List_Details(siisem_it data)
        {
            var result = get_details(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_details(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string code_name = get_codename(id_code_e);
            string sql = "";
            if (code_name == "Laptop" || code_name == "Escritorio")
            {
                sql = "SELECT * FROM siixsem_it_administration.siixsem_processors where name_computer_processing_unit!='N/A'; ";
            }
            else
            {
                sql = "SELECT * FROM siixsem_it_administration.siixsem_processors where name_computer_processing_unit='N/A';";
            }

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_procesador = dr["id"].ToString();
                data.name_procesador = dr["name_computer_processing_unit"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public string get_codename(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "select cod_description from siixsem_code_equipment where IDCODE_E =" + id_code_e;

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.cod_description = dr["cod_description"].ToString();
                con.Close();
                return data.cod_description;
            }
            return "";
        }
        public JsonResult List_DetailsEquipment(siisem_it data)
        {
            var result = get_detailsEquip(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_detailsEquip(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * FROM siixsem_it_administration.siixsem_processors where name_computer_processing_unit !='N/A';";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.iddetails = dr["id"].ToString();
                data.characteristic1 = dr["name_computer_processing_unit"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult List_ram_memory(siisem_it data)
        {
            var result = get_ram_memory(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_ram_memory(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * FROM siixsem_it_administration.siixsem_ram_memory_capacity order by ram_memoria desc ";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_ram_memory = dr["id"].ToString();
                data.ram_memoria = dr["ram_memoria"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }

        public JsonResult List_type_disk(siisem_it data)
        {
            var result = get_type_disk(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_type_disk(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * FROM siixsem_it_administration.siixsem_disc_type";



            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_type_disk = dr["id"].ToString();
                data.name_type_disk = dr["disc_type"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult List_disk_memory(siisem_it data)
        {
            var result = get_disk_memory(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_disk_memory(string id_code_e)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * FROM siixsem_it_administration.siixsem_disk_capacity where disk_capacity!='N/A';";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.disk_capacity = dr["disk_capacity"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }

        public JsonResult List_Departamens()
        {
            var result = get_departaments();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> get_departaments()
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * From siixsem_departament ";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.iddep = dr["iddep"].ToString();
                data.name_dep = dr["name_dep"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public ActionResult usuarios()
        {
            return View();
        }
        public ActionResult equipos_asignados()
        {
            return View();
        }
        public ActionResult responsive_letters()
        {
            return View();
        }
        public JsonResult List_EquipD(siisem_it data)
        {
            var result = Get_EquipD();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> Get_EquipD()
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "CALL EQUIPOS_DISPONIBLES()";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.e_name = dr["e_name"].ToString();
                data.code_description = dr["cod_description"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }

        public JsonResult Get_Brands(siisem_it data)
        {
            var result = List_brands(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> List_brands(string id)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            //sql = "SELECT id_code_e, brand FROM siixsem_it_administration.siixsem_brand_e WHERE id_code_e = '" + id + "' group by brand";
            sql = "	SELECT DISTINCT id_trademark,name_trademark FROM siixsem_it_administration.siixsem_trademark_model T0 " +
                  " INNER JOIN siixsem_it_administration.siixsem_trademarks T1 on T0.id_trademark = T1.id " +
                  " where id_code_equipment =" + id;

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_code_e = dr["id_trademark"].ToString();
                data.brand = dr["name_trademark"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult Get_Brands_Carg(siisem_it data)
        {
            var result = List_brands_C(data.id_code_e);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> List_brands_C(string id)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT id_code_e, brand FROM siixsem_it_administration.siixsem_brand_e WHERE id_code_e = '" + id + "' group by brand";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_code_e = dr["id_code_e"].ToString();
                data.brand = dr["brand"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult Get_Models(siisem_it data)
        {
            var result = List_models(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<siisem_it> List_models(siisem_it datos)
        {
            if (datos.brand == null) { datos.brand = "1"; }
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";

            sql = "SELECT * FROM siixsem_it_administration.siixsem_trademark_model T0 " +
                    " INNER JOIN siixsem_it_administration.siixsem_models T2 on T0.id_model = T2.id" +
                    " where id_code_equipment = " + datos.id_code_e + " and id_trademark = " + datos.brand +
                    " order by name_model asc ";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id_model"].ToString();
                data.model_name = dr["name_model"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult Get_DetailsE(siisem_it data)
        {
            var datos = Get_DetailsEqui(data.id);
            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_DetailsU(siisem_it data)
        {
            var datos = Get_DetailsUser(data.id);
            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        public List<siisem_it> Get_DetailsEqui(string id)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT " +
                         " T0.id,t0.id_eq,t0.e_name,t0.series,t0.e_status,t0.warranty,t0.comments,t0.ubication,t0.activo_fijo,t0.bitlocker," +
                         " T2.name_trademark, " +
                         " T3.name_model," +
                         " T4.name_dep, " +
                         " T5.idcode_e AS ID_CODIGO_EQUIPO, T5.cod_description, " +
                         " T7.id as id_type_disk ,T7.disc_type, " +
                         " T8.id as id_almacenamiento,T8.disk_capacity , " +
                         " t9.id as id_ram,T9.ram_memoria, " +
                         " T6.id as id_procesador,T10.name_computer_processing_unit" +
                         " FROM siixsem_it_administration.siixsem_equipment T0 " +
                         " INNER JOIN siixsem_it_administration.siixsem_trademark_model T1 ON T0.id_trademark_model = T1.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_trademarks T2 ON T1.id_trademark = T2.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_models T3 ON T1.id_model = T3.id  " +
                         " INNER JOIN siixsem_it_administration.siixsem_departament T4 ON T0.id_department = T4.iddep " +
                         " INNER JOIN siixsem_it_administration.siixsem_code_equipment T5 ON T0.id_code_e = T5.idcode_e " +
                         " INNER JOIN siixsem_it_administration.siixsem_details_equipment T6 ON T0.id_details_equipment = T6.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_disc_type T7 ON T6.id_disc_type = T7.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_disk_capacity T8 ON T6.id_disk_capacity = T8.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_ram_memory_capacity T9 ON T6.id_ram_memory = T9.id " +
                         " INNER JOIN siixsem_it_administration.siixsem_processors T10 ON T6.id_processor = T10.id " +
                         " WHERE T0.id='" + id + "'";


            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.e_name = dr["e_name"].ToString();
                data.series = dr["series"].ToString();
                data.model_name = dr["name_model"].ToString();
                data.name_dep = dr["name_dep"].ToString();
                data.warranty = dr["warranty"].ToString();
                data.comments = dr["comments"].ToString();
                data.activo_fijo = dr["activo_fijo"].ToString();
                data.bitlocker = dr["bitlocker"].ToString();
                data.ubication = dr["ubication"].ToString();
                data.e_status = dr["e_status"].ToString();
                data.brand = dr["name_trademark"].ToString();
                data.id_code_e = dr["ID_CODIGO_EQUIPO"].ToString();
                data.cod_description = dr["cod_description"].ToString();
                data.id_procesador = dr["id_procesador"].ToString();
                data.name_procesador = dr["name_computer_processing_unit"].ToString();
                data.id_almacenamiento = dr["id_almacenamiento"].ToString();
                data.disk_capacity = dr["disk_capacity"].ToString();
                data.id_type_disk = dr["id_type_disk"].ToString();
                data.name_type_disk = dr["disc_type"].ToString();
                data.id_ram_memory = dr["id_ram"].ToString();
                data.ram_memoria = dr["ram_memoria"].ToString();

                list.Add(data);
            }
            con.Close();

            return (list);
        }

        public List<siisem_it> Get_DetailsUser(string id)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_users T0" +
                         " WHERE T0.id='" + id + "'";


            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.name = dr["name"].ToString();
                data.lastname = dr["lastname"].ToString();
                data.username = dr["username"].ToString();
                data.iddep = dr["id_department"].ToString();
                data.type_user = dr["type_user"].ToString();
                data.no_nomi = dr["no_nomi"].ToString();


                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult Get_HistorycEquip(siisem_it data)
        {
            var datos = List_HistoryE(data.id);
            return Json(datos, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> List_HistoryE(string id)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT t1.id, t1.id_eq,t2.username, t2.no_nomi,t0.status, t0.name_file FROM siixsem_it_administration.siixsem_files_responsive t0 inner join siixsem_equipment t1 on t1.id = t0.id_equip inner join siixsem_users t2 on t2.id = t0.id_user where t1.id = '" + id + "'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.username = dr["username"].ToString();
                data.no_nomi = dr["no_nomi"].ToString();
                data.e_status = dr["status"].ToString();
                data.name_file = dr["name_file"].ToString();


                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public string Carta_Responsiva_Vigente(string id)
        {
            siisem_it data;
            
            string sql = "SELECT t1.id, t1.id_eq,t2.username, t2.no_nomi,t0.status, t0.name_file FROM siixsem_it_administration.siixsem_files_responsive t0 inner join siixsem_equipment t1 on t1.id = t0.id_equip inner join siixsem_users t2 on t2.id = t0.id_user where t1.id = '" + id + "' and status='Vigente'";

            siixsem_connection c2 = new siixsem_connection();
            string conexion = c2.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.id = dr2["id"].ToString();
                data.id_eq = dr2["id_eq"].ToString();
                data.username = dr2["username"].ToString();
                data.no_nomi = dr2["no_nomi"].ToString();
                data.e_status = dr2["status"].ToString();
                data.name_file = dr2["name_file"].ToString();
                con2.Close();

                return data.name_file;
            }
            con2.Close();
            return "";

        }
        public JsonResult Get_DetailsEquipAsig(siisem_it data) {

            //  var result = Update_Equipment(data);
            string user = Session["id"].ToString();
            //  string identificador = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult update_equipment_it(siisem_it data)
        {
            var result = Update_Equipment(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Update_Equipment(siisem_it data)
        {
            string id_details_equipment = get_id_details_equipment(data);

            string sql = "UPDATE siixsem_equipment SET e_status='" + data.e_status + "',comments='" + data.comments + "',ubication='" + data.ubication + "', id_details_equipment='" + id_details_equipment + "', bitlocker = '"+ data.bitlocker + "', activo_fijo = '"+data.activo_fijo +"' where id='" + data.id + "'";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }
        //---------------------------INVOICES--------------------------------//
        public JsonResult GetInvoice()
        {
            var result = GetInvoice_db();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> GetInvoice_db()
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_invoice;";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.idsiixsem_invoice = int.Parse(dr["idsiixsem_invoice"].ToString());
                data.id_invoice = dr["id_invoice"].ToString();
                data.date_invoice = dr["date_invoice"].ToString();
                data.name_file = dr["name_file"].ToString();
                list.Add(data);
            }
            con.Close();
            return (list);
        }
        public JsonResult Add_Invoice(siisem_it data)
        {
            var result = Add_InvoiceBD(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Add_InvoiceBD(siisem_it data)
        {
            int id = 1;
            try
            {
                string sqlid = "SELECT id_invoice, date_invoice FROM siixsem_invoice;";
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sqlid, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                while (dr.Read())
                {
                    id++;
                }
                con.Close();

                string sql = "INSERT INTO `siixsem_it_administration`.`siixsem_invoice` (`idsiixsem_invoice`,`date_invoice`, `id_invoice`) VALUES ("+id+",'" + data.date_invoice + "', '" + data.id_invoice + "');";
                commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "Factura Añadida Exitosamente!";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
        [HttpPost]
        public ActionResult DetailsInvoice(string idinv)
        {
            ViewBag.idinv = idinv;
            return View();
        }
        public JsonResult Get_DetailsInvoice(string id_invoice)
        {
            var result = GetInvoiceD_db(id_invoice);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> GetInvoiceD_db(string id_invoice)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT siixsem_equipment.id_eq, siixsem_equipment.series FROM siixsem_equipment JOIN siixsem_invoice_detail ON siixsem_equipment.id = siixsem_invoice_detail.id_eq " +
                        "JOIN siixsem_invoice ON siixsem_invoice_detail.id_invoice = siixsem_invoice.idsiixsem_invoice " +
                        "WHERE siixsem_invoice.id_invoice = '"+id_invoice+"'; ";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id_eq = dr["id_eq"].ToString();
                data.series = dr["series"].ToString();
                list.Add(data);
            }
            con.Close();
            return (list);
        }
        //--------------------------------USERS------------------------------------///
        public JsonResult GetUsers_List()
        {
            string user = Session["id"].ToString();
            string type_user = Session["type_user"].ToString();

            var result = All_userslist(user, type_user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> All_userslist(string user, string type_user)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";
            switch (type_user) {
                case "Administrador":
                    sql = "SELECT T0.*,T1.* FROM siixsem_it_administration.siixsem_users T0 inner JOIN siixsem_it_administration.siixsem_departament T1  ON t1.iddep = T0.id_department; ";
                    break;
                default:
                    sql = "SELECT T0.*,T1.* FROM siixsem_it_administration.siixsem_users T0 inner JOIN siixsem_it_administration.siixsem_departament T1  ON t1.iddep = T0.id_department WHERE t0.id=" + user + "; ";
                    break;
            }


            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.name = dr["name"].ToString();
                data.lastname = dr["lastname"].ToString();
                data.username = dr["username"].ToString();
                data.name_dep = dr["name_dep"].ToString();
                data.no_nomi = dr["no_nomi"].ToString();
                data.type_user = dr["type_user"].ToString();
                data.date_creation = dr["date_creation"].ToString();

                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public JsonResult add_users(siisem_it data)
        {
            var result = Add_User(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Add_User(siisem_it data)
        {
            try {
                /*ENCRIPTACIOND DE CONTRASEÑA (TOMAREMOS EL NÚMEOR DE NÓMINA)*/
                MD5 md5 = MD5CryptoServiceProvider.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = md5.ComputeHash(encoding.GetBytes(data.password));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);

                string sql = "INSERT INTO `siixsem_it_administration`.`siixsem_users` (`name`, `lastname`, `username`,`password`, `id_department`, `type_user`,`date_creation`,`no_nomi`) VALUES ('" + data.name + "', '" + data.lastname + "', '" + data.name + "." + data.lastname + "','" + sb.ToString() + "', '" + data.iddep + "', '" + data.type_user + "',now(),'" + data.no_nomi + "');";
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "Usuario Añadido Exitosamente!";
            }
            catch (Exception e) {
                return e.ToString();
            }

        }
        public JsonResult update_u(siisem_it data)
        {
            var result = Update_User(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Update_User(siisem_it data)
        {
            try
            {
                string sql = "UPDATE `siixsem_it_administration`.`siixsem_users` SET `name`= '"+data.name+ "', `lastname`='" + data.lastname + "', `id_department`='" + data.iddep + "', `no_nomi`= '" + data.no_nomi + "' WHERE id = '" + data.id + "';";
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "Datos de Usuario Modificados Exitosamente!";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        public JsonResult update_pass(siisem_it data)
        {
            var result = Update_User_Pass(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Update_User_Pass(siisem_it data)
        {
            try
            {
                MD5 md5 = MD5CryptoServiceProvider.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = md5.ComputeHash(encoding.GetBytes(data.password));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);

                string sql = "UPDATE `siixsem_it_administration`.`siixsem_users` SET `password` = '" + sb.ToString() + "' WHERE id = '" + data.id + "';";
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "Datos de Usuario Modificados Exitosamente!";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        [HttpPost]
        public ActionResult Get_DetailsR(string id, string name, string ape, string name_dep, string nomi, HttpPostedFileBase file, string id_equipo, string identificador)
        {
            ViewBag.id = id;
            ViewBag.name = name;
            ViewBag.ape = ape;
            ViewBag.name_dep = name_dep;
            ViewBag.nomi = nomi;
            return View();
        }


        [HttpGet]
        public ActionResult Get_DetailsResponsive(siisem_it data, string id, string name, string ape, string name_dep, string nomi, string id_equipo, string identificador)
        {
            if (data.file != null && data.file.ContentLength > 0)
                try
                {
                    siisem_it datos = new siisem_it();
                    datos.ide = identificador;
                    datos.username = data.id;
                    var lista = Disp_Asignados(datos);
                    foreach (var i in lista)
                    {
                       // update_FileResponsive(data.file, i.id);
                    }

                    string path = Path.Combine(Server.MapPath("~/QR-TIN-004-00 Carta responsiva"),
                                               Path.GetFileName(data.file.FileName));
                    data.file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    return View("Get_DetailsR");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View("Get_DetailsR");
                }
            return View();

        }
        public JsonResult DetailR(siisem_it data)
        {
            var result = Get_ResponsiveL(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> Get_ResponsiveL(siisem_it datos)
        {

            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            //     string code_name = get_codename(datos.id);
            string user = Session["id"].ToString();
            string type_user = Session["type_user"].ToString();
            string sql = "";
            if (datos.ide == null) {
                if (datos.id != null)
                {
                    sql = "SELECT t1.id, t0.idfile, t0.name_file,t0.status,t0.identificador,T1.comments, T1.id_eq,t1.e_name,t1.series,t2.name_dep, t4.name, t4.lastname,t3.cod_description,T6.name_trademark,T7.name_model" +
                             " FROM siixsem_it_administration.siixsem_files_responsive T0" +
                             " INNER JOIN  siixsem_it_administration.siixsem_equipment T1 ON T1.id = T0.id_equip" +
                             " INNER JOIN  siixsem_it_administration.siixsem_departament T2 ON T1.id_department = T2.iddep" +
                             " INNER JOIN siixsem_it_administration.siixsem_code_equipment T3 ON T1.id_code_e = T3.idcode_e" +
                             " INNER JOIN siixsem_it_administration.siixsem_users T4 ON T4.id = T0.id_user " +
                             " INNER JOIN siixsem_it_administration.siixsem_trademark_model T5 ON T1.id_trademark_model = T5.id" +
                             " INNER JOIN siixsem_it_administration.siixsem_trademarks T6 ON T5.id_trademark = T6.id " +
                             " INNER JOIN siixsem_it_administration.siixsem_models T7 ON T5.id_model = T7.id" +
                             " where t0.id_user = " + datos.id + " ORDER BY T0.IDENTIFICADOR ASC";
                } else { sql = sql = "CALL Equipos_por_usuario(" + user + ")"; }

            } else {
                if (type_user == "Administrador") {
                    /* sql = "SELECT  t0.name_file, t0.status,t0.identificador,t1.id, T1.id_eq,t1.e_name,t1.series,t2.name_dep, t4.name, t4.lastname,t3.cod_description,T6.name_trademark,T7.name_model" +
                             " FROM siixsem_it_administration.siixsem_files_responsive T0" +
                             " INNER JOIN  siixsem_it_administration.siixsem_equipment T1 ON T1.id = T0.id_equip" +
                             " INNER JOIN  siixsem_it_administration.siixsem_departament T2 ON T1.id_department = T2.iddep" +
                             " INNER JOIN siixsem_it_administration.siixsem_code_equipment T3 ON T1.id_code_e = T3.idcode_e" +
                             " INNER JOIN siixsem_it_administration.siixsem_users T4 ON T4.id = T0.id_user" +
                             " INNER JOIN siixsem_it_administration.siixsem_trademark_model T5 ON T1.id_trademark_model = T5.id" +
                                  " INNER JOIN siixsem_it_administration.siixsem_trademarks T6 ON T5.id_trademark = T6.id " +
                                  " INNER JOIN siixsem_it_administration.siixsem_models T7 ON T5.id_model = T7.id" +
                             " where t0.id_user = " + datos.id + " and t0.identificador=" + datos.ide;*/

                    //sql = "CALL QR_TIN_004_LetterResponsive("+ datos.id + ","+ datos.ide + ","+datos.sem+")";
                    if (datos.id == "undefined") {
                        sql = "CALL QR_TIN_004_LetterResponsive(" + user + "," + datos.ide + "," + datos.sem + ")";
                    } else {
                        sql = "CALL QR_TIN_004_LetterResponsive(" + datos.id + "," + datos.ide + "," + datos.sem + ")";
                    }
                } else { sql = "CALL QR_TIN_004_LetterResponsive(" + user + "," + datos.ide + "," + datos.sem + ")"; }

            }



            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.idfile = dr["idfile"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.e_name = dr["e_name"].ToString();
                data.series = dr["series"].ToString();
                data.name_dep = dr["name_dep"].ToString();
                data.name = dr["name"].ToString();
                data.name_file = dr["name_file"].ToString();
                data.lastname = dr["lastname"].ToString();
                data.cod_description = dr["cod_description"].ToString();
                data.name_model = dr["name_model"].ToString();
                data.e_status = dr["status"].ToString();
                data.ide = dr["identificador"].ToString();
                data.comments = dr["comments"].ToString();
                data.name_trademark = dr["name_trademark"].ToString();

                /*
                if (type_user == "Administrador") {
                    data.newide = getNewIde(datos.id); }*/

                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public string getNewIde(string user)
        {
            
            siixsem_connection connection2 = new siixsem_connection();
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT identificador FROM siixsem_it_administration.siixsem_files_responsive where id_user = " + user + " ORDER by idfile DESC LIMIT 1";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.ide = dr2["identificador"].ToString();
                con2.Close();
                return data.ide;
            }
            return "";
        }
        public ActionResult PDF_Responsive(string id, string identificador, string id_sem)
        {

            siisem_it obj = new siisem_it
            {
                id = id,
                ide = identificador,
                sem = id_sem
            };
            QR_TIN_004_LetterResponsive rpt = new QR_TIN_004_LetterResponsive();
            rpt.Load();
            var result = Get_ResponsiveL(obj);
            rpt.SetDataSource(result);
            Stream s = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(s, "application/pdf");
        }
        public ActionResult Dowland_Responsive(string name_file)
        {
            var FileVirtualPath = "~/QR-TIN-004-00 Carta responsiva/" + name_file;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));

        }

        public ActionResult Download_Invoice(string name_file)
        {
            var FileVirtualPath = "~/Invoices/" + name_file;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));

        }

        [HttpPost]
        public ActionResult responsive_letters(HttpPostedFileBase file, string idM, string ie_eqM)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    // update_FileResponsive(file, idM, ie_eqM,"");
                    string path = Path.Combine(Server.MapPath("~/QR-TIN-004-00 Carta responsiva"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    return View("responsive_letters");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View("responsive_letters");
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                return View("responsive_letters");
            }
        }

        public void update_FileResponsive(string file, string sem, string identificador, string id)
        {
            string user = Session["id"].ToString();
            string type_user = Session["type_user"].ToString();
            string sql = "";
            /*
            if (type_user == "Administrador")
            {
                sql = "UPDATE  `siixsem_it_administration`.`siixsem_files_responsive` SET  NAME_FILE='" + file + "' WHERE idfile='" + id + "'  ;";
            }
            else
            {
                //  
            }*/
            sql = "UPDATE  `siixsem_it_administration`.`siixsem_files_responsive` SET  NAME_FILE='" + file + "' WHERE ID_EQUIP='" + sem + "' and identificador='" + identificador + "' and id_user='" + id + "' ;";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
        }

        public JsonResult add_responsiveuser(siisem_it data)
        {
            
            var result = Asignar_Equipo(get_tipo_solicitud(data.id) ,data);
     //     Update_Status(data);
            return Json(JsonRequestBehavior.AllowGet);
        }
        public string Asignar_Equipo(string tipo_solicitud,siisem_it data)
        {
            DateTime fecha_actual = DateTime.Now;
            string anio = fecha_actual.Year.ToString();
            string mes = fecha_actual.Month.ToString();
            string day = fecha_actual.Day.ToString();
            int newidentifi = Convert.ToInt32(data.newide) + 1;
            int version = Convert.ToInt32(get_version(data.id_eq)) + 1;
            string user = Session["id"].ToString();
            string sql = "";

            if (tipo_solicitud == "") {
                Add_NewRespU(data);
                Update_Status(data);
                return "";
            }
            else
            {
                sql = "CALL ASIGNAR_EQUIPOS ('" + tipo_solicitud + "', '" + data.id_eq + "', '" + data.id + "','" + user + "');";
                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "";
            }
            
            
        }

        public string get_tipo_solicitud(string id_eq)
        {
            siixsem_connection connection2 = new siixsem_connection();
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT * FROM siixsem_it_administration.siixsem_hardware_loan where id=" + id_eq + ";";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.tipo_solicitud = dr2["tipo_solicitud"].ToString();
                con2.Close();
                return data.tipo_solicitud;
            }
            return "";
        }
        public string Add_NewRespU(siisem_it data)
        {
            DateTime fecha_actual = DateTime.Now;
            string anio = fecha_actual.Year.ToString();
            string mes = fecha_actual.Month.ToString();
            string day = fecha_actual.Day.ToString();
            int newidentifi = Convert.ToInt32(data.newide) + 1;
            int version = Convert.ToInt32(get_version(data.id_eq)) + 1;
            string sql = "INSERT INTO `siixsem_it_administration`.`siixsem_files_responsive` (`date`, `version`, `status`, `id_equip`, `id_user`, `identificador`) VALUES ('" + anio + "/" + mes + "/" + day + "', '" + version + "', 'Vigente', '" + data.id_eq + "', '" + data.id + "'," + newidentifi + ");";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }
        public string Update_Status(siisem_it data)
        {
            DateTime fecha_actual = DateTime.Now;
            string anio = fecha_actual.Year.ToString();
            string mes = fecha_actual.Month.ToString();
            string day = fecha_actual.Day.ToString();
            int newidentifi = Convert.ToInt32(data.newide) + 1;
            int version = Convert.ToInt32(get_version(data.id_eq)) + 1;
            string sql = "update siixsem_it_administration.siixsem_equipment set ubication='',e_status='Asignado' where id='" + data.id_eq + "';";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }
        public JsonResult cancel_responsive(siisem_it data)
        {
            var lista = Disp_Asignados(data);
            foreach (var i in lista) {
                var result = Cancel_CRActive(i.id);
                UPDATE_AFTER_CRCANCEL(data);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> Disp_Asignados(siisem_it datos)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "";
            if (datos.ide != null) {
                sql = "SELECT * FROM siixsem_it_administration.siixsem_files_responsive t0 inner join siixsem_equipment t1 on t0.id_equip = t1.id where t0.idfile =" + datos.ide + " and id_user = " + datos.username + ";";
            }
            else {
                sql = "SELECT * FROM siixsem_it_administration.siixsem_files_responsive t0 inner join siixsem_equipment t1 on t0.id_equip = t1.id where t1.id_eq = " + datos.id_eq + "   and id_user = " + datos.username + "; ";
            }



            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["idfile"].ToString();

                list.Add(data);
            }
            con.Close();

            return (list);
        }
        public string Cancel_CRActive(string id)
        {
            string sql = "update siixsem_it_administration.siixsem_files_responsive set status='No Vigente' where idfile='" + id + "';";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }
        public string UPDATE_AFTER_CRCANCEL(siisem_it datos)
        {
            string sql = "update siixsem_it_administration.siixsem_equipment set ubication='Almacen IT',e_status='Resguardo IT' where id='" + datos.id + "';";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }
        public string get_version(string id_eq)
        {

            siixsem_connection connection2 = new siixsem_connection();
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT count(*)+1 as contador FROM siixsem_it_administration.siixsem_files_responsive where id_equip=" + id_eq + ";";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.ide = dr2["contador"].ToString();
                con2.Close();
                return data.ide;
            }
            return "";
        }
        [HttpPost]
        public ActionResult Dowland(siisem_it data) {
            var FileVirtualPath = "~/QR-TIN-004-00 Carta responsiva/" + data.id_eq;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));

        }

        // GET: siixsem_it/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: siixsem_it/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: siixsem_it/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: siixsem_it/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: siixsem_it/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: siixsem_it/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: siixsem_it/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult IT_services_request ()
        {
            return View();
        }
        public JsonResult newline_tbody_user(siisem_it conta)
        {
            int numero = conta.Contador - 1;
            string resultado2 = "<tr>" +
                    " <td><center><td><center><input type = 'checkbox'/></td></center></td><td><center> <input class='form-control' id='series_" + numero + "' name='series' /></center></td><td><center><select id='brad_" + numero + "' class='form-control'></select ></center></td><td><center><select id='model_" + numero + "' class='form-control'></center></td></tr>";
            string resultado = "<tr><td><center><input type = 'checkbox' /></td>" +
                                    "<td><center><input type = 'checkbox' /></center></td>" +
                                    "<td><center><input type = 'text' class='form-control' id='nomina_" + numero + "' /></center></td>" +
                                     "<td><center><input type = 'text' class='form-control' id='nombre_" + numero + "' /></center></td>" +
                                      "<td><center><input type = 'text' class='form-control' id='apellido_" + numero + "' /></center></td>" +
                                      "<td><center><select class='form-control' id='departamento'></select></td>" +
                                      "</tr>";

            return Json(resultado);
        }
        public ActionResult loan_hardware()
        {
            return View();
        }

        public JsonResult Add_Loan_Equipment(siisem_it data)
        {
               var result2 = Add_LoanEquipment(data);
               string user = Session["username"].ToString();

               var result = Notificacion_Solicitudes(user, data.tipo_solicitud,get_departamento(), data.fecha_requerida ,get_id_hardware_load());
                         


            return Json(result2, JsonRequestBehavior.AllowGet);
        }
        public string get_departamento()
        {
            string id = Session["id_department"].ToString();
            siixsem_connection connection2 = new siixsem_connection();
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT name_dep  FROM siixsem_it_administration.siixsem_users t0  inner join siixsem_it_administration.siixsem_departament T1 on t0.id_department=T1.iddep where t0.id_department=" + id + ";";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.departamento = dr2["name_dep"].ToString();
                con2.Close();
                return data.departamento;
            }
            return "";
        }
        public string Add_LoanEquipment(siisem_it data)
        {
            try {
                string user = Session["id"].ToString();
                DateTime fecha_actual = DateTime.Now;
                string anio = fecha_actual.Year.ToString();
                string mes = fecha_actual.Month.ToString();
                string day = fecha_actual.Day.ToString();
                string fecha_solicitud = anio + "/" + mes + "/" + day;
                string sql = "INSERT INTO `siixsem_it_administration`.`siixsem_hardware_loan` (`laptop`, `Escanner`, `impresora_wifi`, `Estatus`,`comentarios`, `id_user_solicitante`, `id_user_presto_it`, `id_user_entrega`, `id_user_recibio_it`, `fecha_que_solicitan`,`fecha_requerida`, `fecha_que_se_presta`, `fecha_que_entregan`,`tipo_solicitud`)  " +
                             "VALUES ('" + data.laptop + "', '" + data.escanners + "', '" + data.impresoras + "', "+"'Enviado','" + data.comments + "', '" + user + "', '116', '116', '116', '" + fecha_solicitud + "','" + data.fecha_requerida + "', '0000-00-00', '0000-00-00','"+data.tipo_solicitud+"');";

                siixsem_connection c = new siixsem_connection();
                string conexion = c.conection_bd();
                con = new MySqlConnector.MySqlConnection(conexion);
                MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
                con.Open();
                dr = commandDatabase.ExecuteReader();
                con.Close();
                return "OK";
            } catch (Exception e) {
                return e.ToString();
            }
        }
        public string get_id_hardware_load()
        {
            string id = Session["id"].ToString();
            siixsem_connection connection2 = new siixsem_connection();
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string sql = "SELECT MAX(id) AS id FROM siixsem_it_administration.siixsem_hardware_loan where id_user_solicitante=" + id + ";";

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con2 = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con2);
            con2.Open();
            dr2 = commandDatabase.ExecuteReader();
            while (dr2.Read())
            {
                data = new siisem_it();
                data.id = dr2["id"].ToString();
                con2.Close();
                return data.id;
            }
            return "";
        }
        /*-------------------------------------------------------------------------------------------------NOTIFICACION PARA PRESTAMO DE HARWARE-------------------------------------------------------------------------------------------------------------------*/
        public string Notificacion_Solicitudes(string usuario, string solicitud, string departamento, string fecha_necesaria, string id_solicitud)
        {
            try
            {
                DateTime fecha_actual = DateTime.Now;

                string receptor = "siix.flow@siix-global.com";
                string Diana = "it-sem@siix-global.com";
            //    string jhona = "arturo.hernandez@siix-global.com";


                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.office365.com");

                mail.From = new MailAddress(receptor, "IT", Encoding.UTF8); //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía


                mail.Subject = "I N T R A N E T  IT Solicitud de Hardware " + id_solicitud;
                //Aquí ponemos el mensaje que incluirá el corre
                mail.Body = "Solicitud #: " + id_solicitud + "\n\r" + "Creada por : " + usuario + "\n\r" + "Solicitud requerida: " + solicitud + "\n\r" + "Departamento: " + departamento + "\n\r" + "Fecha de solicitud : " + fecha_actual + "\n\r" + "Fecha de requerida : " + fecha_necesaria + "\n\r" + "Verifica Intranet de IT Siix Sem para mas detalles";

                mail.To.Add(receptor);
                mail.CC.Add(Diana);
            //    mail.CC.Add(jhona);
                //Si queremos enviar archivos adjuntos tenemos que especificar la ruta en donde se encuentran
                // mail.Attachments.Add(new Attachment(@"C:\Users\Sistemas\Pictures\BOTON_PAG_WEB.png"));

                //Configuracion del SMTP
                SmtpServer.Port = 587; //Puerto que utiliza Hostinger para sus servicios

                SmtpServer.Credentials = new System.Net.NetworkCredential(receptor, "Top19846"); //Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }

            catch (Exception e)
            {
                return Convert.ToString(e);
            }

            return "";

        }

        public JsonResult Get_Hardware_loan(siisem_it data)
        {
            var result = ListHardware_loan(data.id,data.ide,data.iddetails);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public List<siisem_it> ListHardware_loan(string id,string solicitud,string iddetails)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string user = Session["id"].ToString();
           
            string sql = "";
             if (solicitud==null) { 
                sql = "CALL QUERY_SOLICITUDES_PRESTAMO_EQUIPOS(" + user + ",0) ;"; 
            } else {
                if (iddetails != null) { sql = "CALL QUERY_SOLICITUDES_PRESTAMO_EQUIPOS(" + iddetails + "," + solicitud + ") ;"; } else {sql = "CALL QUERY_SOLICITUDES_PRESTAMO_EQUIPOS(" + user + ","+solicitud+") ;"; }
                
            }
            

            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.laptop = dr["laptop"].ToString();
                data.escanners = dr["escanner"].ToString();
                data.impresoras = dr["impresora_wifi"].ToString();
                data.comments = dr["comentarios"].ToString();
                data.e_status = dr["estatus"].ToString();
                data.id_user_solicitante = dr["id_user_solicitante"].ToString();
                data.id_user_presto_it = dr["id_user_presto_it"].ToString();
                data.id_user_entrega = dr["id_user_entrega"].ToString();
                data.id_user_recibio_it = dr["id_user_recibio_it"].ToString();
                data.fecha_que_solicitan = dr["fecha_que_solicitan"].ToString();
                data.fecha_requerida = dr["fecha_requerida"].ToString();
                data.fecha_que_se_presta = Convert.ToString(dr["fecha_que_se_presta"]) ;
                data.fecha_que_entregan = dr["fecha_que_entregan"].ToString();
                data.tipo_solicitud = dr["tipo_solicitud"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }

        public ActionResult Formato_Prestamo_Equipo(string solicitud)
        {
            string id = Session["id"].ToString();
            siisem_it obj = new siisem_it
            {
                id = id,
                ide = solicitud
            };
            RELACION_DE_EQUIPO_EN_PRESTAMO rpt = new RELACION_DE_EQUIPO_EN_PRESTAMO();
            rpt.Load();
            var result = List_Detail_Hardware_loan(obj);
            rpt.SetDataSource(result);
            Stream s = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(s, "application/pdf");
        }

        public List<siisem_it> List_Detail_Hardware_loan(siisem_it obj)
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
            string user = Session["id"].ToString();
            string type_user = Session["type_user"].ToString();
            string sql = ""; 
            if (type_user=="Administrador") { 
                
                if (obj.id != null) { 
                    if (obj.id_user_solicitante==null) { 
                        sql = "CALL FORMATO_PRESTAMO_EQUIPOS('" + obj.id + "','" + obj.ide + "','0') ;"; 
                    } else 
                    { 
                        sql = "CALL FORMATO_PRESTAMO_EQUIPOS('" + obj.id_user_solicitante + "','" + obj.id + "','" + obj.id_eq + "') ;"; 
                    }
                } 
                else { sql = "CALL FORMATO_PRESTAMO_EQUIPOS(" + obj.iddetails + "," + obj.ide + ",0) ;"; }

            }
            else {

                 sql = "CALL FORMATO_PRESTAMO_EQUIPOS(" + user + ","+obj.ide+",0) ;"; 
            }
            
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.fecha_que_solicitan = dr["fecha_que_solicitan"].ToString();
                data.fecha_que_se_presta = Convert.ToString(dr["fecha_que_se_presta"]);
                data.fecha_que_entregan = dr["fecha_que_entregan"].ToString();
                data.comentarios = dr["comentarios"].ToString();
                data.comments = dr["comentarios_dispositivos"].ToString();
                data.e_status= dr["estatus_dispositivo"].ToString();
                data.id_eq = dr["id_eq"].ToString();
                data.series = dr["series"].ToString();
                data.code_name = dr["code_name"].ToString();
                data.cod_description = dr["cod_description"].ToString();
                data.name_trademark = dr["name_trademark"].ToString();
                data.name_model = dr["name_model"].ToString();
                data.id_user_solicitante = dr["id_user_solicitante"].ToString();
                data.id_user_entrega = dr["id_user_entrega"].ToString();
                data.id_user_presto_it = dr["id_user_presto_it"].ToString();
                data.id_user_recibio_it = dr["id_user_recibio_it"].ToString();
                data.departamento = dr["departamento"].ToString();
                list.Add(data);
            }
            con.Close();
            return (list);
        }
        public ActionResult Asignacion_hardware()
        {
            return View();
        }
        public JsonResult Get_Solicitudes_Harware(siisem_it data)
        {
            var result = List_Solicitudes_Harware();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<siisem_it> List_Solicitudes_Harware()
        {
            siisem_it data;
            List<siisem_it> list = new List<siisem_it>();
           
            string sql = "";
            sql = "CALL  QUERY_SOLICITUDES_PRESTAMO_EQUIPOS(0,0);";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            while (dr.Read())
            {
                data = new siisem_it();
                data.id = dr["id"].ToString();
                data.laptop = dr["laptop"].ToString();
                data.escanners = dr["escanner"].ToString();
                data.impresoras = dr["impresora_wifi"].ToString();
                data.comments = dr["comentarios"].ToString();
                data.e_status = dr["estatus"].ToString();
                data.iddetails= dr["id_user_s"].ToString(); 
                data.id_user_solicitante = dr["id_user_solicitante"].ToString();
                data.id_user_presto_it = dr["id_user_presto_it"].ToString();
                data.id_user_entrega = dr["id_user_entrega"].ToString();
                data.id_user_recibio_it = dr["id_user_recibio_it"].ToString();
                data.fecha_que_solicitan = dr["fecha_que_solicitan"].ToString();
                data.fecha_que_se_presta = Convert.ToString(dr["fecha_que_se_presta"]);
                data.fecha_que_entregan = dr["fecha_que_entregan"].ToString();
                data.fecha_requerida = dr["fecha_requerida"].ToString();
                data.tipo_solicitud = dr["tipo_solicitud"].ToString();
                list.Add(data);
            }
            con.Close();

            return (list);
        }


        [HttpPost]
        public ActionResult Detail_Solicitud_Hardware(string id,string id_user_solicitante)
        {
            ViewBag.id = id;
            ViewBag.id_user_solicitante = id_user_solicitante;
            
            return View();
        }

        public JsonResult DetailR_(siisem_it data)
        {
            var result = List_Detail_Hardware_loan(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetailEquip_A(siisem_it data)
        {
            var result = List_Detail_Hardware_loan(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult update_details_load_hardware(siisem_it data)
        {
            var result = Update_details_equipment(data.id,data.e_status,data.comments);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string Update_details_equipment(string id, string e_status,string comentarios)
        {
            
            string sql =  "UPDATE `siixsem_it_administration`.`siixsem_details_hardware_loan` SET `comentarios` = '"+ comentarios + "', `Estatus` = '"+ e_status + "' WHERE (`id` = '"+id+"');\r\n";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
            return "";
        }


        [HttpPost]
        public ActionResult UploadFiles(siisem_it data)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/QR-TIN-004-00 Carta responsiva"), fname);
                        file.SaveAs(fname);
                        update_FileResponsive(file.FileName, data.sem,data.identificador,data.id);
                        
                        /*
                        siisem_it data = new siisem_it();
                        datos.ide = data.ide;
                        datos.username = data.id;
                        var lista = Disp_Asignados(datos);
                        foreach (var x in lista)
                        {
                            update_FileResponsive(file, x.id);
                        }*/
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public void update_FileInvoice(string file, int id)
        {
            string sql = "";
            sql = "UPDATE  `siixsem_it_administration`.`siixsem_invoice` SET  name_file ='" + file + "' WHERE idsiixsem_invoice = "+id+";";
            siixsem_connection c = new siixsem_connection();
            string conexion = c.conection_bd();
            con = new MySqlConnector.MySqlConnection(conexion);
            MySqlConnector.MySqlCommand commandDatabase = new MySqlConnector.MySqlCommand(sql, con);
            con.Open();
            dr = commandDatabase.ExecuteReader();
            con.Close();
        }

        [HttpPost]
        public ActionResult UploadInvoice(siisem_it data)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Invoices"), fname);
                        file.SaveAs(fname);
                        update_FileInvoice(file.FileName, data.idsiixsem_invoice);

                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intranet_siix_sem.Clases
{
    public class siisem_it
    {
        public string id { get; set; }
        public string id_eq { get; set; }
        public string e_name { get; set; }
        public string model_name { get; set; }
        public string series { get; set; }
        public string name_dep { get; set; } 
        public string warranty { get; set; }
        public string id_procesador { get; set; }
        public string name_procesador { get; set; }
        public string iddetails { get; set; }
        public string id_ram_memory { get; set; }
        public string ram_memoria { get; set; }
        
        public string comments { get; set; }
        public string comentarios { get; set; }
        public string ubication { get; set; }
        public string e_status { get; set; }
        public string characteristic1 { get; set; }
        public string characteristic2 { get; set; }
        public string characteristic3 { get; set; }
        public string brand { get; set; }
        public string cod_description { get; set; }
        public string reception_date { get; set; }
        public string delivery_date { get; set; }
        public string it_user_receiving { get; set; }
        public string it_user_who_prepares { get; set; }
        public string id_code_e { get; set; }
        public string code_name { get; set; }
        public string code_description { get; set; }
        public string id_type_disk { get; set; }
        public string name_type_disk { get; set; }
        public string iddep { get; set; }
        public string name { get; set; }
        public string name_file { get; set; }
        public string activo_fijo { get; set; }
        public string bitlocker { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string type_user { get; set; }
        public string date_creation { get; set; }
        public string no_nomi { get; set; }
        public int Contador { get; set; }
        public string ide { get; set; }
        public string newide { get; set; }
        public string identificador { get; set; }
        public string sem { get; set; }
        public string id_almacenamiento { get; set; }
        public string disk_capacity { get; set; }
        public string laptop { get; set; }
        public string escanners { get; set; }
        public string id_user_solicitante { get; set; }
        public string id_department { get; set; }
        public string departamento { get; set; }
        public string id_user_presto_it { get; set; }
        public string id_user_entrega { get; set; }
        public string id_user_recibio_it { get; set; }
        public string impresoras { get; set; }
        public string fecha_que_solicitan { get; set; }
        public string fecha_requerida { get; set; }
        public string fecha_que_se_presta { get; set; }
        public string fecha_que_entregan { get; set; }
        public string tipo_solicitud { get; set; }
        public string name_trademark { get; set; }
        public string name_model { get; set; }
        public string commentarios { get; set; }
        public string idfile { get; set; }
        //Invoices
        public int idsiixsem_invoice { get; set; }
        public string id_invoice { get; set; }
        public string date_invoice { get; set; }
        public int idsiixsem_invoice_detail { get; set; }
        public HttpPostedFileBase file { get; set; }

    }
}
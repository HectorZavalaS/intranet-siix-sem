using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intranet_siix_sem.Clases
{
    public class siixsem_connection
    {
        public string conection_bd() {
            string server = "192.168.3.13";
            string database = "siixsem_it_administration";
            string user = "root";
            string password = "S3m4dm1n2017!";
            string port = "3306";
            string sslM = "none";
            string connString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5};Allow Zero Datetime=True", server, port, user, password, database, sslM);
  
            return connString;
        }
    }
}
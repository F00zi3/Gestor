using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapaDatos
{
    public static class Conexion
    {
        private static string server = @"DESKTOP-DIB3CRQ\SQLEXPRESS01";
        private static string db = @"GestorJuegos";
        private static string usuario = @"tig8";
        private static string clave = @"contraseña";

        public static string DarStrConexion()
        {
            return string.Concat(@"Data Source=", server, ";Initial Catalog=", db,
                ";Persist Security Info=False;User ID=", usuario, ";Password=", clave);
        }

    }
}

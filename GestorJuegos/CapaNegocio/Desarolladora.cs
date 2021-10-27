using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CapaDatos;


namespace CapaNegocio
{
    public class Desarolladora
    {
        private int id;
        private string nombre;
        private string sede;
        private int añosexp;

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Sede { get => sede; set => sede = value; }
        public int Añosexp { get => añosexp; set => añosexp = value; }

        public Desarolladora(int id, string nombre, string sede, int añosexp)
        {
            this.id = id;
            this.nombre = nombre;
            this.sede = sede;
            this.añosexp = añosexp;
        }
        public Desarolladora()
        {
            id = 0;
            nombre = sede = "";
            añosexp = 0;

        }
        public override string ToString()
        {
            return string.Concat(nombre, " ", sede, " ID = ", id, "Exp =", añosexp);
        }
        public void Guardar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            eDesarolladora p = new eDesarolladora();

            if (this.id == 0) //esto detecta que es un objeto nuevo
            {
                CargarFilaDesarolladora(p);
                dc.eDesarolladora.InsertOnSubmit(p);

                foreach (PaisVisitado pv in paisesVisitados)
                {
                    pv.Guardar(dc, p);
                }

            }
            else //este caso es para una fila que viene de la DB, id != 0
            {
                p = (from x in dc.eDesarolladora where x.id == this.id select x).FirstOrDefault();
                CargarFilaDesarolladora(p);
                foreach (PaisVisitado pv in paisesVisitados)
                {
                    pv.Guardar(dc, p);
                }
            }

            dc.SubmitChanges();
        }
        private void CargarFilaDesarolladora(eDesarolladora p)
        {
            p.nombre = this.nombre;
            p.sede = this.sede;
            p.añosexp = this.añosexp;
            p.fktipodesarolladora = this.tipodesarolladora.Id;
        }
        public static List<Desarolladora> Buscar(string buscado)
        {
            List<Desarolladora> resultados = new List<Desarolladora>();
            buscado = buscado.ToLower();

            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var filas = from x in dc.eDesarolladora
                        where x.añoexp.ToLower().Contains(buscado) ||
                        x.nombre.ToLower().Contains(buscado) ||
                        x.sede.ToLower().Contains(buscado) ||
                        x.eTipodesarolladora.tipo.ToLower().Contains(buscado)
                        select x;

            if (filas != null)
            {
                foreach (var f in filas)
                {
                    resultados.Add(new Desarolladora(f.id, f.nombre, f.sede, f.añoexp, Genero.BuscarPorId(f.fktipodesarolladora)));
                }
            }

            return resultados;

        }
        public void Eliminar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var enc = (from x in dc.eDesarolladora where x.id == this.id select x).FirstOrDefault();
            if (enc != null)
            {
                dc.eDesarolladora.DeleteOnSubmit(enc);
                dc.SubmitChanges();
            }
            else
            {
                throw new Exception("No se pudo eliminar el dato, no fue encontrado el id: " + this.id);
            }
        }
        public static Desarolladora BuscarPorId(int id)
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var f = (from x in dc.eDesarolladora where x.id == id select x).FirstOrDefault();
            Desarolladora p = new Desarolladora(f.id, f.nombre, f.sede, f.añoexp, Genero.BuscarPorId(f.fktipodesarolladora));

            return p;
        }
        public static DataTable BuscarDT(string buscado)
        {
            SqlConnection sqlConn = new SqlConnection(Conexion.DarStrConexion()); //Creamos la conexión
            try
            {
                sqlConn.Open(); //la abrimos
                SqlDataAdapter adapter; //este adaptador permite enviar la consulta y obtener los resultados usando la conexion
                DataSet ds = new DataSet(); //el dataset es donde vamos a guardar los resultados, cada resultado es un DataTable (una tabla completa)

                string consulta =
                    string.Concat("select ",
                                  "p.id, p.nombre 'Nombre', p.sede 'Sede', p.añoexp 'Año de experiencia', pa.tipodesarolladora 'Tipo de Desarolladora' ",
                                  "from Desarolladora p ",
                                  "inner join Desarolladora pa on p.fktipodesarolladora = pa.id ",
                                  "where p.nombre like '%", buscado, "%' or p.sede like '%", buscado, "%' or p.añoexp like '%", buscado, "%' or pa.nombre like '%", buscado, "%'");

                adapter = new SqlDataAdapter(consulta, sqlConn); //cargamos en el adaptador la consulta y la conexión

                adapter.Fill(ds); //ejecutamos y cargamos los resultados en el dataset
                return ds.Tables[0]; //retornamos el DataTable N°0 que es el primero que se llena, esto es si solo tiene 1 tabla, si tiene mas de una, uso DataSet como tipo de dato a retornar
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConn.Close(); //Cerramos la conexión
                sqlConn.Dispose(); //Liberamos los recursos
            }

        }
    }
}

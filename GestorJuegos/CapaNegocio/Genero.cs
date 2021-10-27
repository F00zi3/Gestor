using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CapaDatos;


namespace CapaNegocio
{
    public class Genero
    {
        private int id;
        private string nombre;

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public override bool Equals(object obj)
        {
            return obj is Genero genero &&
                   Nombre == genero.Nombre;
        }
        public Genero(string nombre, int id)
        {
            this.Id = id;
            this.Nombre = nombre;

        }
        public Genero()
        {
            Id = 0;
            Nombre = "";
        }
        public override string ToString()
        {
            return string.Concat(Nombre, " ", " ID = ", Id);
        }
        public void Guardar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            eGenero p = new eGenero();

            if (this.Id == 0) //esto detecta que es un objeto nuevo
            {
                CargarFilaGenero(p);
                dc.eGenero.InsertOnSubmit(p);

            }
            else //este caso es para una fila que viene de la DB, id != 0
            {
                p = (from x in dc.eGenero where x.id == this.Id select x).FirstOrDefault();
                CargarFilaGenero(p);
                foreach (PaisVisitado pv in paisesVisitados)
                {
                    pv.Guardar(dc, p);
                }
            }

            dc.SubmitChanges();
        }
        private void CargarFilaGenero(eGenero p)
        {
            p.nombre = this.Nombre;
            p.id = this.Id;
        }
        public static List<Genero> Buscar(string buscado)
        {
            List<Genero> resultados = new List<Genero>();
            buscado = buscado.ToLower();

            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var filas = from x in dc.eGenero
                        where x.detalle.ToLower().Contains(buscado) ||
                        x.nombre.ToLower().Contains(buscado) ||
                        x.id.ToLower().Contains(buscado) 
                        select x;

            if (filas != null)
            {
                foreach (var f in filas)
                {
                    resultados.Add(new Genero(f.id, f.nombre));
                }
            }

            return resultados;

        }
        public void Eliminar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var enc = (from x in dc.eGenero where x.id == this.Id select x).FirstOrDefault();
            if (enc != null)
            {
                dc.eGenero.DeleteOnSubmit(enc);
                dc.SubmitChanges();
            }
            else
            {
                throw new Exception("No se pudo eliminar el dato, no fue encontrado el id: " + this.Id);
            }
        }
        public static Genero BuscarPorId(int id)
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var f = (from x in dc.eGenero where x.id == id select x).FirstOrDefault();
            Genero p = new Genero(f.id, f.nombre);

            return p;
        }
    }
}

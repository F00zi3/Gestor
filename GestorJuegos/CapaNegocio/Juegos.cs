using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CapaDatos;

namespace CapaNegocio
{
   public class Juegos
    {
        private int id;
        private string nombre;
        private string detalle;
        private DateTime fecLan;
        private Genero genero;

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Detalle { get => detalle; set => detalle = value; }
        public DateTime FecLan { get => fecLan; set => fecLan = value; }
        internal Genero Genero { get => genero; set => genero = value; }

        public Juegos(int id, string nombre, string detalle, DateTime fecLan, Genero genero)
        {
            this.id = id;
            this.nombre = nombre;
            this.detalle = detalle;
            this.fecLan = fecLan;
            this.genero = genero;
        }
        public Juegos()
        {
            id = 0;
            nombre = detalle = "";
            fecLan = new DateTime(1900, 1, 1);
            genero = null;
        }
        public override string ToString()
        {
            return string.Concat(nombre, " ", detalle, " ID = ", id);
        }

        public void Guardar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            eJuegos p = new eJuegos();

            if (this.id == 0) //esto detecta que es un objeto nuevo
            {
                CargarFilaPersona(p);
                dc.eJuegos.InsertOnSubmit(p);

                foreach (PaisVisitado pv in paisesVisitados)
                {
                    pv.Guardar(dc, p);
                }

            }
            else //este caso es para una fila que viene de la DB, id != 0
            {
                p = (from x in dc.eJuegos where x.id == this.id select x).FirstOrDefault();
                CargarFilaJuegos(p);
                foreach (PaisVisitado pv in paisesVisitados)
                {
                    pv.Guardar(dc, p);
                }
            }

            dc.SubmitChanges();
        }
        private void CargarFilaJuegos(eJuegos p)
        {
            p.nombre = this.nombre;
            p.detalle = this.detalle;
            p.fecLan = this.fecLan;
            p.fkgenero = this.genero.Id;
        }

        public static List<Juegos> Buscar(string buscado)
        {
            List<Juegos> resultados = new List<Juegos>();
            buscado = buscado.ToLower();

            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var filas = from x in dc.eJuegos
                        where x.detalle.ToLower().Contains(buscado) ||
                        x.nombre.ToLower().Contains(buscado) ||
                        x.fecLan.ToString().Contains(buscado) ||
                        x.eGenero.nombre.ToLower().Contains(buscado)
                        select x;

            if (filas != null)
            {
                foreach (var f in filas)
                {
                    resultados.Add(new Juegos(f.id, f.nombre, f.detalle, f.fecLan, Genero.BuscarPorId(f.fkGenero)));
                }
            }

            return resultados;

        }
        public void Eliminar()
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var enc = (from x in dc.eJuegos where x.id == this.id select x).FirstOrDefault();
            if (enc != null)
            {
                dc.eJuegos.DeleteOnSubmit(enc);
                dc.SubmitChanges();
            }
            else
            {
                throw new Exception("No se pudo eliminar el dato, no fue encontrado el id: " + this.id);
            }
        }
        public static IQueryable BuscarIQ(string buscado)
        {
            buscado = buscado.ToLower();

            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var filas = from x in dc.eJuegos
                        where x.detalle.ToLower().Contains(buscado) ||
                        x.nombre.ToLower().Contains(buscado) ||
                        x.fecLan.ToString().Contains(buscado) ||
                        x.eGenero.nombre.ToLower().Contains(buscado)
                        select new
                        {
                            Id = x.id,
                            Juegos = x.nombre.ToUpper() + ", " + x.detalle.ToUpper(),
                            Lanzamiento = x.fecLan,
                            Genero = x.eGenero.nombre.ToUpper(),
                            

                        };
            return filas;
        }
        public static Juegos BuscarPorId(int id)
        {
            DCDataContext dc = new DCDataContext(Conexion.DarStrConexion());
            var f = (from x in dc.eJuegos where x.id == id select x).FirstOrDefault();
            Juegos p = new Juegos(f.id, f.nombre, f.detalle, f.fecLan, Genero.BuscarPorId(f.fkgenero));

            return p;
        }

        
    }
}

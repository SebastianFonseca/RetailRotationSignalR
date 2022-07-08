using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class EgresoModel
    {
        public int id { get; set; }
        public decimal valor { get; set; }
        public ProveedorModel proveedor { get; set; } = new ProveedorModel();
        public string descripcion { get; set; }
        public ItemMovimientoEfectivoModel itemMovimientoefectivo { get; set; } = new ItemMovimientoEfectivoModel();
        public EmpleadoModel responsable { get; set; } = new EmpleadoModel();
        public DateTime fecha { get; set; }
        public LocalModel local { get; set; } = new LocalModel();

        public string soporte { get; set; }
        public ProductoModel producto { get; set; } = new ProductoModel();


    }
}

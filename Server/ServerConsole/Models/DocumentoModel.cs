using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class DocumentoModel: Screen
    {
        public string codigo { get; set; }
        public DateTime fecha { get; set; }
        public EmpleadoModel responsable { get; set; } = new EmpleadoModel();
        public LocalModel puntoVenta { get; set; } = new LocalModel();
        public BindableCollection<ProductoModel> productos { get; set; }

    }
}

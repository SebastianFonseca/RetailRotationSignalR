using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public abstract class DocumentoModel : Screen
    {
        public string codigo { get; set; }
        public DateTime fecha { get; set; }
        public EmpleadoModel responsable { get; set; }
        public LocalModel puntoVenta { get; set; }
        public BindableCollection<ProductoModel> productos { get; set; }

    }
}

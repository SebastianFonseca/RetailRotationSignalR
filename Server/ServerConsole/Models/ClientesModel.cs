using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ClientesModel : PersonModel
    {
        public decimal puntos { get; set; }
        public string estado { get; set; }
        public BindableCollection<FacturaModel> facturas { get; set; } = new BindableCollection<FacturaModel>();
    }
}

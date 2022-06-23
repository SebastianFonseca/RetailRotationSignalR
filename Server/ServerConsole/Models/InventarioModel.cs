using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class InventarioModel
    {
        public string codigo { get; set; }

        public LocalModel local { get; set; } = new LocalModel();
    }
}

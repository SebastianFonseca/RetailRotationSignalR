using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class ProveedorModel : PersonModel
    {
        public string ciudad { get; set; }
        public BindableCollection<ProductoModel> productos { get; set; }


    }

}

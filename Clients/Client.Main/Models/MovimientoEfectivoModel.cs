using System;
using System.Collections.Generic;
using System.Text;
using Caliburn;
using Caliburn.Micro;

namespace Client.Main.Models
{
    public class MovimientoEfectivoModel:Screen
    {
        public int id { get; set; }
        public decimal aumentoDisminucion { get; set; }
        public decimal total { get; set; }
        public int? codIngreso { get; set; }
        public EgresoModel egreso { get; set; }
        public LocalModel local { get; set; }

        /*public string tipo { get; set; }
        public DateTime fecha { get; set; }
        ProveedorModel proveedor { get; set; }
        public decimal valor { get; set; }*/

        /// <summary>
        ///Propiedad necesaria para el binding de datos en las listbox con el movimientoseleccionado.
        /// </summary>
        bool _isSelected;
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }

    }
}

﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.Models
{
    public class RecibidoModel : DocumentoModel
    {

        public DateTime fechaRecibido { get; set; } = DateTime.Now.Date;
        public string nombreConductor { get; set; }
        public string placas { get; set; }
        public decimal? numeroCanastillas { get; set; } = null;
        public int? peso { get; set; } = null;

        public BindableCollection<ProductoModel> productosActualizados { get; set; } = new BindableCollection<ProductoModel>();

        public static implicit operator RecibidoModel(EnvioModel v)
        {
            return new RecibidoModel() 
            {
                codigo = v.codigo,
                productos = v.productos,
                puntoVenta = v.puntoVenta,
                responsable = v.responsable,
                placas = v.placasCarro

            };
        }
    }
}

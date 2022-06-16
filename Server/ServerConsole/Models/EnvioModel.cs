﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerConsole.Models
{
    public class EnvioModel : DocumentoModel
    {
        public DateTime fechaEnvio { get; set; } = DateTime.Now.Date;
        public string nombreConductor { get; set; }
        public string placasCarro { get; set; }
        public int? numeroCanastillas { get; set; } = null;
        public int? peso { get; set; } = null;

        public static implicit operator EnvioModel(PedidoModel v)
        {
            return new EnvioModel
            {
                codigo = v.codigo + ":" + DateTime.Now.ToString("ddMMyyyyHHmm"),
                productos = v.productos,
                puntoVenta = v.puntoVenta,
                responsable = v.responsable
            };
        }
    }
}
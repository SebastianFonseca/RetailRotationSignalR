using Autofac;
using Caliburn.Micro;
using Client.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Client.Main.ViewModels
{
    class InformesBuscarProveedoresResultadoBusquedaViewModel : Screen
    {
        public ProveedorModel proveedor;

        public MainWindowViewModel VentanaPrincipal;
        public Connect conexion = ContainerConfig.scope.Resolve<Connect>();

        public InformesBuscarProveedoresResultadoBusquedaViewModel(MainWindowViewModel ventanaPrincipal, ProveedorModel proveedor)
        {
            this.proveedor = proveedor;
            this.VentanaPrincipal = ventanaPrincipal;
      
        }

    }
}


/*
			select rc.CantidadComprada, rc.CedulaProveedor,rc.CodigoProducto,rc.Estado, rc.PrecioCompra, p.Nombre, p.UnidadCompra,c.FechaCompra as fehacompra, pp.fecha as fechapago, pp.soporte, pp.valor from 
			(select CodigoProducto, CantidadComprada, PrecioCompra, Estado,CodigoCompra, CedulaProveedor from RegistroCompra where CedulaProveedor = '5565') rc
			left join 
			Producto p on rc.CodigoProducto = p.CodigoProducto
			left join 
			compras c on c.CodigoCompra = rc.CodigoCompra
		    left join 
			pagosproveedorregistrocompra as pr on rc.CodigoCompra = pr.codigocompra and pr.codigoproducto = rc.codigoproducto
			left join 
			pagosproveedores as pp on pp.id = pr.pagoproveedor order by rc.Estado, c.FechaCompra 

 */
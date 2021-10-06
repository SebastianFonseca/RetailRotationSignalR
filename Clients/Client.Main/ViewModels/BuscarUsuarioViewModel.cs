using Caliburn.Micro;
using Client.Main.Models;
using Client.Main.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Client.Main.ViewModels
{
    class BuscarUsuarioViewModel: PropertyChangedBase, IDataErrorInfo
    {
        MainWindowViewModel VentanaPrincipal;
        public BuscarUsuarioViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
        }


        private string _buscarTbx;

        public string BuscarTbx
        {
            get { return _buscarTbx; }
            set 
            {                 
                _buscarTbx = value;
                NotifyOfPropertyChange(() => BuscarTbx);
            }
        }

        
        private string _busquedasVisibiliad = "Hidden";

        public string BusquedasVisibilidad
        {
            get {
                if (String.IsNullOrEmpty(BuscarTbx) ) { return "Hidden"; }
                return _busquedasVisibiliad; 
            }
            set { _busquedasVisibiliad = value;  NotifyOfPropertyChange(() => BusquedasVisibilidad); }
        }

        private PersonModel _usuarioSeleccionado;

        public PersonModel UsuarioSeleccionado
        {
            get { return _usuarioSeleccionado; }
            set
            {
                
                _usuarioSeleccionado = value;
                BuscarTbx = UsuarioSeleccionado.Cedula+" - "+ UsuarioSeleccionado.FirstName+" "+UsuarioSeleccionado.LastName;
                NotifyOfPropertyChange(() => UsuarioSeleccionado);
            }
        }



        public void ItemSeleccionado()
        {
            BusquedasVisibilidad = "Hidden";
            //_comboboxDesplegado = "false";
         
            //BuscarTbx = BuscarTbx + UsuarioSeleccionado.FirstName;
            

           
        }

        private string _comboboxDesplegado = "false";

        public string ComboboxDesplegado
        {
            get { return _comboboxDesplegado; }
            set { _comboboxDesplegado = value;
                NotifyOfPropertyChange(() => ComboboxDesplegado);
            }
        }



        private BindableCollection<PersonModel> _busquedas = new BindableCollection<PersonModel>();

        public BindableCollection<PersonModel> Busquedas 
        {
            get 
            { 
                return _busquedas;
            }
            set
            {
                    _busquedas = value;
                    NotifyOfPropertyChange(() => Busquedas);                
            }
        }

        public void EscribiendoBusqueda()
        {      
            
            Busquedas.Clear();           
            Busquedas = DbConnection.getEmpleados(BuscarTbx);
            if (Busquedas.Count == 0)
            {
                BusquedasVisibilidad = "Hidden";
            }
            else
            {
                BusquedasVisibilidad = "Visible";
                ComboboxDesplegado = "true";

            }

        }

        public void Buscar() { } 

        public string Error { get { return null; } }
        int flag = 0;
        public string this[string name]
        {
            get
            {
                string result = null;
                if (flag == 1)
                {
                    if (name == "BuscarTbx")
                    {
                        if (String.IsNullOrEmpty(BuscarTbx))
                        {
                            result = "Rellene este campo.";
                        }
                    }                    
                }

                else { flag += 1; }


                return result;

            }
        }

        public void BackButton()
        {
            
            VentanaPrincipal.ActivateItem(new DC_AdministrativoViewModel(VentanaPrincipal));
            Busquedas.Clear();
        }
    }
}

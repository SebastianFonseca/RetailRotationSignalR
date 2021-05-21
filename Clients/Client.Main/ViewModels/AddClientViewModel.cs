 using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Client.Main.Utilities;
using Client.Main.Models;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Client.Main.ViewModels
{
    public class AddClientViewModel : PropertyChangedBase, IDataErrorInfo
    {
        ClientesModel NuevoCliente;
        MainWindowViewModel VentanaPrincipal;

        public AddClientViewModel(MainWindowViewModel argVentana)
        {
            VentanaPrincipal = argVentana;
            NuevoCliente = new ClientesModel();
        }

       
        public string Name
        {
            get { return NuevoCliente.FirstName; }
            set{
                if (NuevoCliente.FirstName != value)
                    NuevoCliente.FirstName = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        public string Apellidos
        {
            get { return NuevoCliente.LastName; }
            set
            {               
                if (NuevoCliente.LastName != value)
                    NuevoCliente.LastName = value;
                NotifyOfPropertyChange(() => Apellidos);
            }
        }

        public string CC
        {
            get { return NuevoCliente.Cedula; }
            set
            {     
                if (NuevoCliente.Cedula != value)
                    NuevoCliente.Cedula = value;
                NotifyOfPropertyChange(() => CC);
            }
        }

        public string Correo
        {
            get { return NuevoCliente.Correo; }
            set
            {
                if (NuevoCliente.Correo != value)
                    NuevoCliente.Correo = value;
                NotifyOfPropertyChange(() => Correo);
            }
        }

        public string Telefono
        {
            get { return NuevoCliente.Telefono; }
            set
            {
                if (NuevoCliente.Telefono != value)
                    NuevoCliente.Telefono = value;
                NotifyOfPropertyChange(() => Telefono);

            }
        }                      
        public void Guardar()
        {
            if (!string.IsNullOrWhiteSpace(NuevoCliente.FirstName) && !string.IsNullOrWhiteSpace(NuevoCliente.LastName) && !string.IsNullOrWhiteSpace(NuevoCliente.Cedula)) 
            {
                //if (string.IsNullOrEmpty(NuevoCliente.Correo))
                //{
                //    NuevoCliente.Correo = null;
                //}
                //if (string.IsNullOrEmpty(NuevoCliente.Telefono))
                //{
                //    NuevoCliente.Telefono = null;
                //}


                try
                {
                    if (DbConnection.AddClient(Cliente:NuevoCliente))
                    {                 
                        VentanaPrincipal.ActivateItem(new AddClientViewModel(VentanaPrincipal));
                    }
                    else
                    {
                        CC = "";
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }
            else
            {
                MessageBox.Show("Primero debe rellenar los datos.");
            }
        

        }

        public string Error { get { return null; } }
        int flag = 0;        
        public string this[string name]
        {
            get
            {
                string result = null;
                long number = 0;
                if (flag == 4)
                {
                    if (name == "CC")
                    {
                        if (!long.TryParse(CC, out number))
                        {
                            result = "Rellene este campo cc.";
                        }
                    }
                    else if(name == "Name")
                    {
                        if (String.IsNullOrEmpty(Name))
                        {
                            result = "Rellene este campo name.";
                        }
                    }
                    else if (name == "Apellidos")
                    {
                        if (String.IsNullOrEmpty(Apellidos))
                        {
                            result = "Rellene este campo apl.";
                        }

                    }
                    else if (name == "Telefono")
                    {
                        if (Telefono.Length != 10)
                        {
                            result = "El número telefónico debe tener 10 digitos.";
                        }

                    }
                }
                
                else { flag += 1; }


                return result;
            }
        }





    }



}



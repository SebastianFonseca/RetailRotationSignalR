using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Main.ViewModels
{
    public class ShellViewModel : Screen 
    {
        private string _name;

        public  string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WpfAppTest.ViewModel
{
    public class ViewModelBase
    {
        public Commands.SimpleCommand SimpleCommand { get; set; }
        public ViewModelBase()
        {

            this.SimpleCommand = new Commands.SimpleCommand(this);
        }

        public void SimpleMethod()
        {
            Debug.WriteLine("Hello");
        }
    }
}

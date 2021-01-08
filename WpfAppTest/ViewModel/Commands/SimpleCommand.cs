using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfAppTest.ViewModel.Commands
{
    public class SimpleCommand : ICommand
    {
        public ViewModelBase ViewModel { get; set; }
        public SimpleCommand(ViewModelBase viewModel)
        {
            this.ViewModel = viewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.SimpleMethod();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Music_Labrary_Manager
{
    public class Command : ICommand
    {
        private readonly Action<object> execute;

        public Command(Action<object> execute)
        {
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}

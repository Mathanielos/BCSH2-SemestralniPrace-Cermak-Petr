using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BCSH2SemestralniPraceCermakPetr.ViewModels
{
    public class ValidationViewModel : ViewModelBase
    {
        public event EventHandler ValidationCompleted;
        private string message;

        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
        public ValidationViewModel(string msg)
        {
            ExitCommand = ReactiveCommand.Create(Exit);
            Message = msg;
        }
        private void Exit()
        {
            ValidationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}

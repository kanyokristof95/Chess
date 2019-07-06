using System;
using System.Windows.Input;

namespace Chess.ViewModel
{
    public class DelegateCommand : ICommand
    {
        #region Fields

        private readonly Action<Object> _execute; // lambda function
        private readonly Func<Object, Boolean> _canExecute; // Condition of lambda function

        #endregion


        #region Constructor

        public DelegateCommand(Action<Object> execute) : this(null, execute) { }

        public DelegateCommand(Func<Object, Boolean> canExecute, Action<Object> execute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion


        #region Methods of Execute

        public Boolean CanExecute(Object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(Object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        #endregion


        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion


        #region Event handler

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }
}

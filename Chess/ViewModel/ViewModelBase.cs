using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chess.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Event for changing the property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Event handler

        /// <summary>
        /// Check the property changing
        /// </summary>
        /// <param name="propertyName">Property's name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}

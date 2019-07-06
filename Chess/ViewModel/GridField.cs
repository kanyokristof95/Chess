using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Persistence;

namespace Chess.ViewModel
{
    public class GridField : ViewModelBase
    {
        #region Properties

        public int Row { get; set; }

        public char Column { get; set; }

        private Colour _background;

        public Colour Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        private Colour _colour;

        public Colour Colour
        {
            get => _colour;
            set
            {
                _colour = value;
                OnPropertyChanged();
            }
        }

        private Piece _piece;

        public Piece Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                OnPropertyChanged();
            }
        }

        private bool _selected;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        private bool _optional;

        public bool Optional
        {
            get => _optional;
            set
            {
                _optional = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ClickCommand { get; set; }

        #endregion
    }
}

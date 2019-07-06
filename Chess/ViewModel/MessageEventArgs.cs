using System;

namespace Chess.ViewModel
{
    public class MessageEventArgs : EventArgs
    {
        public string Caption { get; private set; }
        
        public string Text { get; private set; }

        public MessageEventArgs(string text, string caption)
        {
            Caption = caption;
            Text = text;
        }
    }
}

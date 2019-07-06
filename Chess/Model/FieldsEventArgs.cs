using System;
using System.Collections.Generic;
using Chess.Persistence;

namespace Chess.Model
{
    public class FieldsEventArgs : EventArgs
    {
        public List<FieldPosition> Fields { get; private set; }

        public FieldsEventArgs(List<FieldPosition> fields)
        {
            Fields = fields;
        }
    }
}

using System.Collections.Generic;
using SadConsole;
using System;
using Microsoft.Xna.Framework;

namespace LofiHollow.UI {
    public class MessageLogWindow : Window { 
        private static readonly int _maxLines = 100;
         
        private readonly Queue<string> _lines;
         
        private SadConsole.ScrollingConsole _messageConsole;
        
        private SadConsole.Controls.ScrollBar _messageScrollBar;
        private int _scrollBarCurrentPosition;
        private int _windowBorderThickness = 2;

        public MessageLogWindow(int width, int height, string title) : base(width, height) {  
            _lines = new Queue<string>();
            CanDrag = false;
            Title = title.Align(HorizontalAlignment.Center, Width, (char) 205);

            _messageConsole = new SadConsole.ScrollingConsole(width - _windowBorderThickness, _maxLines);
            _messageConsole.DefaultBackground = Color.Black;
            _messageConsole.Position = new Point(1, 1);
            _messageConsole.ViewPort = new Rectangle(0, 0, width - 2, height - _windowBorderThickness);

            // create a scrollbar and attach it to an event handler, then add it to the Window
            _messageScrollBar = new SadConsole.Controls.ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorderThickness);
            _messageScrollBar.Position = new Point(_messageConsole.Width + 1, _messageConsole.Position.X);
            _messageScrollBar.IsEnabled = false;
            _messageScrollBar.ValueChanged += MessageScrollBar_ValueChanged;
            Add(_messageScrollBar);
             
            UseMouse = true;
             
            Children.Add(_messageConsole);
        }

        void MessageScrollBar_ValueChanged(object sender, EventArgs e) {
            _messageConsole.ViewPort = new Rectangle(0, _messageScrollBar.Value + _windowBorderThickness, _messageConsole.Width, _messageConsole.ViewPort.Height);
        }

        public void Add(string message) {
            _lines.Enqueue(message);

            if (_lines.Count > _maxLines) {
                _lines.Dequeue();
            }

            _messageConsole.Cursor.Position = new Point(1, _lines.Count);
            _messageConsole.Cursor.Print(message + "\n");
        }

        public override void Update(TimeSpan time) {
            base.Update(time);
             
            if (_messageConsole.TimesShiftedUp != 0 | _messageConsole.Cursor.Position.Y >= _messageConsole.ViewPort.Height + _scrollBarCurrentPosition) { 
                _messageScrollBar.IsEnabled = true;
                 
                if (_scrollBarCurrentPosition < _messageConsole.Height - _messageConsole.ViewPort.Height) 
                    _scrollBarCurrentPosition += _messageConsole.TimesShiftedUp != 0 ? _messageConsole.TimesShiftedUp : 1;
                 
                _messageScrollBar.Maximum = _scrollBarCurrentPosition - _windowBorderThickness;
                 
                _messageScrollBar.Value = _scrollBarCurrentPosition;
                 
                _messageConsole.TimesShiftedUp = 0;
            }
        }
    }
}

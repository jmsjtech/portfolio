using System.Collections.Generic;
using SadConsole;
using System;
using Microsoft.Xna.Framework;

namespace Oasis.UI {
    public class MessageLogWindow : Window {
        private static readonly int _maxLines = 100;

        private readonly Queue<string> _lines;

        private SadConsole.ScrollingConsole _messageConsole;
        //scrollbar for message console
        private SadConsole.Controls.ScrollBar _messageScrollBar;
        private int _scrollBarCurrentPosition;
        private int _windowBorderThickness = 2;


        public MessageLogWindow(int width, int height, string title) : base(width, height) { 
            Theme.FillStyle.Background = DefaultBackground;
            _lines = new Queue<string>();
            CanDrag = false;
            Title = title.Align(HorizontalAlignment.Center, Width, (char) 205);

            // add the message console, reposition, enable the viewport, and add it to the window
            _messageConsole = new SadConsole.ScrollingConsole(width - _windowBorderThickness, _maxLines);
            _messageConsole.Position = new Point(1, 1);
            _messageConsole.ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorderThickness);

            // create a scrollbar and attach it to an event handler, then add it to the Window
            _messageScrollBar = new SadConsole.Controls.ScrollBar(SadConsole.Orientation.Vertical, height - _windowBorderThickness);
            _messageScrollBar.Position = new Point(_messageConsole.Width + 1, _messageConsole.Position.X);
            _messageScrollBar.IsEnabled = false;
            _messageScrollBar.ValueChanged += MessageScrollBar_ValueChanged;
            Add(_messageScrollBar);

            // enable mouse input
            UseMouse = true;

            // Add the child consoles to the window
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

        // Custom Update method which allows for a vertical scrollbar
        public override void Update(TimeSpan time) {
            base.Update(time);

            // Ensure that the scrollbar tracks the current position of the _messageConsole.
            if (_messageConsole.TimesShiftedUp != 0 | _messageConsole.Cursor.Position.Y >= _messageConsole.ViewPort.Height + _scrollBarCurrentPosition) {
                //enable the scrollbar once the messagelog has filled up with enough text to warrant scrolling
                _messageScrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (_scrollBarCurrentPosition < _messageConsole.Height - _messageConsole.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    _scrollBarCurrentPosition += _messageConsole.TimesShiftedUp != 0 ? _messageConsole.TimesShiftedUp : 1;

                // Determines the scrollbar's max vertical position
                // Thanks @Kaev for simplifying this math!
                _messageScrollBar.Maximum = _scrollBarCurrentPosition - _windowBorderThickness;

                // This will follow the cursor since we move the render area in the event.
                _messageScrollBar.Value = _scrollBarCurrentPosition;

                // Reset the shift amount.
                _messageConsole.TimesShiftedUp = 0;
            }
        }
    }
}
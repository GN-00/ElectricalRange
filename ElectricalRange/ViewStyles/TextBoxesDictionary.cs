using System;
using System.Windows;
using System.Windows.Controls;

namespace ProjectsNow.ViewStyles
{
    public partial class TextBoxesDictionary : ResourceDictionary
    {
        private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            _ = textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }
    }
}

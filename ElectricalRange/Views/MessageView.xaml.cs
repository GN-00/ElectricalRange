using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Views
{
    public enum MessageType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum MessageViewImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        None
    }

    public enum MessageViewButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }
    public partial class MessageView : Window
    {
        public MessageView()
        {
            InitializeComponent();
        }

        private static MessageView _messageBox;
        private static MessageBoxResult _result = MessageBoxResult.No;

        public static MessageBoxResult Show
        (string caption, string msg, MessageType type)
        {
            switch (type)
            {
                case MessageType.ConfirmationWithYesNo:
                    return Show(caption, msg, MessageViewButton.YesNo,
                    MessageViewImage.Question);
                case MessageType.ConfirmationWithYesNoCancel:
                    return Show(caption, msg, MessageViewButton.YesNoCancel,
                    MessageViewImage.Question);
                case MessageType.Information:
                    return Show(caption, msg, MessageViewButton.OK,
                    MessageViewImage.Information);
                case MessageType.Error:
                    return Show(caption, msg, MessageViewButton.OK,
                    MessageViewImage.Error);
                case MessageType.Warning:
                    return Show(caption, msg, MessageViewButton.OK,
                    MessageViewImage.Warning);
                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageType type)
        {
            return Show(string.Empty, msg, type);
        }

        public static MessageBoxResult Show(string msg)
        {
            return Show(string.Empty, msg,
            MessageViewButton.OK, MessageViewImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text)
        {
            return Show(caption, text,
            MessageViewButton.OK, MessageViewImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text, MessageViewButton button)
        {
            return Show(caption, text, button,
            MessageViewImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text,
        MessageViewButton button, MessageViewImage image)
        {
            _messageBox = new MessageView
            { txtMsg = { Text = text }, MessageTitle = { Text = caption } };
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            _ = _messageBox.ShowDialog();
            return _result;
        }

        private static void SetVisibilityOfButtons(MessageViewButton button)
        {
            switch (button)
            {
                case MessageViewButton.OK:
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _ = _messageBox.btnOk.Focus();
                    break;
                case MessageViewButton.OKCancel:
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _ = _messageBox.btnCancel.Focus();
                    break;
                case MessageViewButton.YesNo:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _ = _messageBox.btnNo.Focus();
                    break;
                case MessageViewButton.YesNoCancel:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _ = _messageBox.btnCancel.Focus();
                    break;
                default:
                    break;
            }
        }
        private static void SetImageOfMessageBox(MessageViewImage image)
        {
            switch (image)
            {
                case MessageViewImage.Warning:
                    _messageBox.SetImage("Warning.png");
                    break;
                case MessageViewImage.Question:
                    _messageBox.SetImage("Question.png");
                    break;
                case MessageViewImage.Information:
                    _messageBox.SetImage("Information.png");
                    break;
                case MessageViewImage.Error:
                    _messageBox.SetImage("Error.png");
                    break;
                case MessageViewImage.None:
                    _messageBox.img.Visibility = Visibility.Collapsed;
                    break;
                default:
                    _messageBox.img.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
            {
                _result = MessageBoxResult.OK;
            }
            else if (sender == btnYes)
            {
                _result = MessageBoxResult.Yes;
            }
            else if (sender == btnNo)
            {
                _result = MessageBoxResult.No;
            }
            else if (sender == btnCancel)
            {
                _result = MessageBoxResult.Cancel;
            }
            else
            {
                _result = MessageBoxResult.None;
            }

            _messageBox.Close();
            _messageBox = null;
        }

        private void SetImage(string imageName)
        {
            string uri = string.Format("/Images/Icons/{0}", imageName);
            Uri uriSource = new(uri, UriKind.RelativeOrAbsolute);
            img.Source = new BitmapImage(uriSource);
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

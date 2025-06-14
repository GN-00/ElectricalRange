using ProjectsNow.Views;

using System.Collections.Generic;
using System.Windows;

namespace ProjectsNow.Data
{
    public class ViewModelBase : Base
    {
        public Window WindowData { get; set; }
        public IView ViewData { get; protected set; }
        public List<string> AccessKeys { get; protected set; } = new List<string>();

        private bool _IsLoading = false;
        private string _LoadingText = "Loading...";
        private Visibility _LoadingIcon = Visibility.Collapsed;

        public string LoadingText
        {
            get => _LoadingText;
            set => SetValue(ref _LoadingText, value);
        }
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetValue(ref _IsLoading, value);
        }
        public Visibility LoadingIcon
        {
            get => _LoadingIcon;
            set => SetValue(ref _LoadingIcon, value);
        }
    }
}

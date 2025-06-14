using ProjectsNow.Commands;
using ProjectsNow.Data;

using System.Windows;
using System.Windows.Controls.Primitives;

namespace ProjectsNow.Views
{
    public class MainViewModel : Base
    {
        private bool _Home;
        private bool _Back;
        private string _Username;
        private string _Version;
        private IView _CurrentView;
        private bool _OpenPopup;
        private IPopup _PopupView;
        private Popup _MainPopup;

        private string _LoadingText = null;
        private Visibility _LoadingIcon = Visibility.Collapsed;
        private bool _IsLoading = false;

        public MainViewModel(Popup popup)
        {
            MainPopup = popup;
            HomeCommand = new RelayCommand(ToHome, CanToHome);
            BackCommand = new RelayCommand(GoBack, CanGoBack);
        }

        public bool Home
        {
            get => _Home;
            set => SetValue(ref _Home, value);
        }
        public bool Back
        {
            get => _Back;
            set => SetValue(ref _Back, value);
        }
        public string Username
        {
            get => _Username;
            set => SetValue(ref _Username, value);
        }
        public string Version
        {
            get => _Version;
            set => SetValue(ref _Version, value);
        }
        public IView CurrentView
        {
            get => _CurrentView;
            set
            {
                if (SetValue(ref _CurrentView, value))
                {
                    Navigation.CanBack();
                }
            }
        }
        public Popup MainPopup
        {
            get => _MainPopup;
            set => SetValue(ref _MainPopup, value);
        }
        public bool OpenPopup
        {
            get => _OpenPopup;
            set => SetValue(ref _OpenPopup, value);
        }
        public IPopup PopupView
        {
            get => _PopupView;
            set => SetValue(ref _PopupView, value);
        }


        public string LoadingText
        {
            get => _LoadingText;
            set => SetValue(ref _LoadingText, value);
        }
        public Visibility LoadingIcon
        {
            get => _LoadingIcon;
            set => SetValue(ref _LoadingIcon, value);
        }
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetValue(ref _IsLoading, value);
        }


        public RelayCommand HomeCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }

        private void ToHome()
        {
            Navigation.ClosePopup();
            Navigation.To(Navigation.DashboardViewData);
            Navigation.ResetAccessKeys();
        }
        private bool CanToHome()
        {
            if (IsLoading)
                return false;

            return Home;
        }

        private void GoBack()
        {
            Navigation.Back();
        }
        private bool CanGoBack()
        {
            if (Back)
            {
                if (OpenPopup)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
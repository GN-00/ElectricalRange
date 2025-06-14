using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Data
{
    public class Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return false;
            }

            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
    public static class BaseController
    {
        public static bool UpdateProperties(this bool result, Base baseClass, params string[] Properties)
        {
            if (!result)
            {
                return false;
            }

            foreach (string property in Properties)
            {
                baseClass.OnPropertyChanged(property);
            }

            return true;
        }
    }
}

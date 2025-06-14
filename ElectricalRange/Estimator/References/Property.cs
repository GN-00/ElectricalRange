using GN.Base.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Estimator.References
{
    public class Property : ModelBase
    {
        public string Id { get; }
        public int i { get; }
        public int j { get; }


        private string _selected = null;
        public string Selected
        {
            get => _selected;
            set
            {
                if (SetValue(ref _selected, value))
                {
                    OnPropertyChanged(nameof(Image));
                    PropertyListChange?.Invoke(LinkedProperties);
                }
            }
        }

        private string _DefaultValue;
        public string DefaultValue
        {
            get => _DefaultValue;
            set
            {
                if (SetValue(ref _DefaultValue, value))
                    if (value != null)
                        Selected = value;
            }
        }

        public BitmapImage Image
        {
            get => _selected == null ? gray : green;
        }

        private readonly BitmapImage green =
            new(new Uri(@"/Images/Green.png", UriKind.Relative));

        private readonly BitmapImage gray =
            new(new Uri(@"/Images/Gray.png", UriKind.Relative));


        private List<string> _valuse;
        public List<string> Values
        {
            get => _valuse;
            set => SetValue(ref _valuse, value);
        }

        public List<Linked> LinkedProperties { get; set; }

        public Action<List<Linked>> PropertyListChange;

        public Visibility PropertyVisibility => Id != null ? Visibility.Visible : Visibility.Collapsed;

        public override string ToString()
        {
            return $"{i}{j}:{Id}";
        }
    }
}

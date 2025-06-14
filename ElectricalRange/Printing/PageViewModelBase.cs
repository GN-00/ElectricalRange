using ProjectsNow.Data;

using System.Windows.Media;

namespace ProjectsNow.Printing
{
    public class PageViewModelBase : Base
    {
        private int? _PageNumber;
        private int? _TotalPages;
        private ImageSource _BackgroundImage;

        public int? PageNumber
        {
            get => _PageNumber;
            set => SetValue(ref _PageNumber, value);
        }

        public int? TotalPages
        {
            get => _TotalPages;
            set => SetValue(ref _TotalPages, value);
        }

        public ImageSource BackgroundImage
        {
            get => _BackgroundImage;
            set => SetValue(ref _BackgroundImage, value);
        }
    }

}

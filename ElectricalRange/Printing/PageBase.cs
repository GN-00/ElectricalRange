using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectsNow.Printing
{
    public class PageBase : UserControl
    {
        public void SetPage(int pageNumber, int totalPage)
        {
            ((PageViewModelBase)DataContext).PageNumber = pageNumber;
            ((PageViewModelBase)DataContext).TotalPages = totalPage;
        }

        public void SetBackground(ImageSource imageSoure)
        {
            ((PageViewModelBase)DataContext).BackgroundImage = imageSoure;
        }
    }
}

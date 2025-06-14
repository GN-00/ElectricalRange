using System.ComponentModel;
using System.Linq;

namespace ProjectsNow.Commands
{
    public static class DataGridIndicator
    {
        public static string Get(int selectedIndex, ICollectionView collectionView)
        {
            if (collectionView == null)
            {
                return "-";
            }

            if (selectedIndex > -1)
            {
                return $"{selectedIndex + 1} / {collectionView.Cast<object>().Count()}";
            }
            else
            {
                if (collectionView.Cast<object>().Count() > 0)
                {
                    return collectionView.Cast<object>().Count().ToString();
                }
                else
                {
                    return "-";
                }
            }
        }
    }
}

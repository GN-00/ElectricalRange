using ProjectsNow.Data.References;

using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Data.Application
{
    public static class AppData
    {
        public static string Version { get; set; }
        public static decimal VAT { get; set; }
        public static string ComputerName => Environment.MachineName.ToString();
        public static BitmapSource UserWatermark { get; set; }
        public static BitmapSource CompanyWatermark { get; set; }
        public static Company CompanyData { get; set; }
        public static ObservableCollection<Reference> ReferencesListData { get; set; }
        public static ObservableCollection<Brand> BrandsData { get; set; }
        public static ObservableCollection<References.Category> CategoriesData { get; set; }
        public static ObservableCollection<Article1> Articles1Data { get; set; }
        public static ObservableCollection<Article2> Articles2Data { get; set; }


        public static readonly double cm = 37.7952755905512;
    }
}

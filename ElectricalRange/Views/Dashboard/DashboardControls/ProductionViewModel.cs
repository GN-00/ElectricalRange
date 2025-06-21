using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Users;
using ProjectsNow.Views.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsNow.Views.Dashboard.DashboardControls
{


    public class ProductionViewModel : ViewModelBase
    {
        private double _MaxWidth = 1300;
        private User _UserData;

        public ProductionViewModel()
        {
            UserData = Navigation.UserData;
            NewCommand = new RelayCommand(New, CanAccessNew);
            JobOrdersCommand = new RelayCommand(JobOrders, CanAccessJobOrders);

            int buttons = 0;
            if (CanAccessNew()) buttons += 1;
            if (CanAccessJobOrders()) buttons += 1;

            if (buttons == 4) MaxWidth = 900;
        }

        public double MaxWidth
        {
            get => _MaxWidth;
            set => SetValue(ref _MaxWidth, value);
        }
        public User UserData
        {
            get => _UserData;
            set => SetValue(ref _UserData, value);
        }

        public RelayCommand NewCommand { get; }
        public RelayCommand JobOrdersCommand { get; }

        public void New()
        {
            Navigation.To(new OrdersNew());
        }
        public bool CanAccessNew()
        {
            return UserData.AccessNewJobOrder;
        }

        public void JobOrders()
        {
            Navigation.To(new OrdersView());
        }
        public bool CanAccessJobOrders()
        {
            return UserData.AccessJobOrders;
        }
    }
}

using Dapper;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Quotations;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace ProjectsNow.Views.QuotationsViews
{
    internal class TargetPriceViewModel : ViewModelBase
    {
        public TargetPriceViewModel(Quotation quotation, ObservableCollection<QPanel> panels)
        {
            QuotationData = quotation;
            PanelsData = panels;
            NewData = QuotationData.QuotationEstimatedPrice;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public decimal? NewData { get; set; }
        public Quotation QuotationData { get; }
        public ObservableCollection<QPanel> PanelsData { get; private set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Save()
        {
            if (PanelsData.Sum(p => p.PanelsPrice) == 0)
            {
                _ = MessageView.Show("Price Error", "Can't change zero price!!", MessageViewButton.OK, MessageViewImage.Warning);

                return;
            }

            if (PanelsData.Where(p => p.PanelType != "Ready Made Panel").Count() != 0)
            {
                string query = "";
                decimal quotationPrice = PanelsData.Sum(p => p.PanelsPrice) * (1m - QuotationData.Discount / 100m);
                decimal readyMadePanelsPrice = PanelsData.Where(p => p.PanelType == "Ready Made Panel").Sum(p => p.PanelsPrice) * (1m - QuotationData.Discount / 100m);
                decimal otherpanelsPrice = PanelsData.Where(p => p.PanelType != "Ready Made Panel").Sum(p => p.PanelsPrice) * (1m - QuotationData.Discount / 100m);

                decimal targetPriceWithoutReadyMadePanels = NewData.GetValueOrDefault() - readyMadePanelsPrice;

                decimal pp = targetPriceWithoutReadyMadePanels / otherpanelsPrice;

                foreach (QPanel panelData in PanelsData.Where(p => p.PanelType != "Ready Made Panel" && p.PanelsPrice != 0))
                {
                    panelData.PanelProfit = 100m - 100m / pp * (1 - panelData.PanelProfit / 100m);
                    query += $"Update [Quotation].[QuotationsPanels] set PanelProfit = {panelData.PanelProfit} Where PanelID = {panelData.PanelID}; ";
                }

                using SqlConnection connection = new(Database.ConnectionString);
                _ = connection.Execute(query);
            }

            Navigation.ClosePopup();
        }
        private bool CanSave()
        {
            if (NewData == null)
                return false;

            if (NewData > QuotationData.QuotationEstimatedPrice * 1.2m)
                return false;

            if (NewData < QuotationData.QuotationEstimatedPrice * 0.8m)
                return false;

            return true;
        }

        private void Cancel()
        {
            Navigation.ClosePopup();
        }
        private bool CanCancel()
        {
            return true;
        }
    }
}
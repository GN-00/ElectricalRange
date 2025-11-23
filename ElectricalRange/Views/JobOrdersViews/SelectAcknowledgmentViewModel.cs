using Dapper;

using Microsoft.Data.SqlClient;

using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Printing;
using ProjectsNow.Windows.JobOrderWindows;

using System.Windows;

namespace ProjectsNow.Views.JobOrdersViews
{
    internal class SelectAcknowledgmentViewModel : ViewModelBase
    {
        private string _ReportType = "Factory";
        private bool _IsSite = false;

        public SelectAcknowledgmentViewModel(JobOrder jobOrder, IView checkPoint)
        {
            ViewData = checkPoint;
            JobOrderData = jobOrder;

            PrintCommand = new RelayCommand(Print, CanAccessPrint);
            EditCommand = new RelayCommand(Edit, CanAccessEdit);

            AttachCommand = new RelayCommand<JobOrder>(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand<JobOrder>(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand<JobOrder>(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand<JobOrder>(ReadAttachment, CanAccessReadAttachment);
        }


        public bool IsSite
        {
            get => _IsSite;
            set {
                SetValue(ref _IsSite, value);
                if (_IsSite)
                    ReportType = "Site";
                else
                    ReportType = "Factory";
            }
        }
        public string ReportType
        {
            get => _ReportType;
            set => SetValue(ref _ReportType, value);
        }
        public User UserData => Navigation.UserData;
        public JobOrder JobOrderData { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand<JobOrder> AttachCommand { get; }
        public RelayCommand<JobOrder> DeleteAttachmentCommand { get; }
        public RelayCommand<JobOrder> DownloadAttachmentCommand { get; }
        public RelayCommand<JobOrder> ReadAttachmentCommand { get; }

        private void Print()
        {
            MessageBoxResult result = MessageView.Show($"Printing",
                                                       $"Print with watermark?",
                                                       MessageViewButton.YesNo, MessageViewImage.Question);

            string query;
            AcknowledgmentInformation acknowledgementInformationData;
            using (SqlConnection connection = new(Database.ConnectionString))
            {
                query = $"Select * From [JobOrder].[AcknowledgmentsInformations] Where JobOrderID = {JobOrderData.ID}";
                acknowledgementInformationData = connection.QueryFirstOrDefault<AcknowledgmentInformation>(query);
            }

            if (IsSite)
            {
                SiteOrderAcknowledgement acknowledgementForm = new()
                {
                    AcknowledgementInformationData = acknowledgementInformationData
                };
                if (result == MessageBoxResult.Yes)
                {
                    acknowledgementForm.BackgroundImage.Visibility = Visibility.Visible;
                }
                FrameworkElement element = acknowledgementForm;
                Printing.Print.PrintPreview(element, $"Order Acknowledgement-{JobOrderData.Code}", ViewData);
            }
            else
            {
                OrderAcknowledgement acknowledgementForm = new()
                {
                    AcknowledgementInformationData = acknowledgementInformationData
                };
                if (result == MessageBoxResult.Yes)
                {
                    acknowledgementForm.BackgroundImage.Visibility = Visibility.Visible;
                }
                FrameworkElement element = acknowledgementForm;
                Printing.Print.PrintPreview(element, $"Order Acknowledgement-{JobOrderData.Code}", ViewData);
            }

        }
        private bool CanAccessPrint()
        {
            return true;
        }


        private void Edit()
        {
            Navigation.ClosePopup();
            if (Navigation.UserData.Access(JobOrderData))
            {
                string query;
                Acknowledgment acknowledgement;
                using (SqlConnection connection = new(Database.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[Acknowledgment] Where JobOrderID = {JobOrderData.ID}";
                    acknowledgement = connection.QueryFirstOrDefault<Acknowledgment>(query);
                }

                if (IsSite)
                {
                    SiteAcknowledgementWindow acknowledgementWindow = new()
                    {
                        UserData = Navigation.UserData,
                        AcknowledgementData = acknowledgement,
                    };
                    _ = acknowledgementWindow.ShowDialog();
                }
                else
                {
                    AcknowledgementWindow acknowledgementWindow = new()
                    {
                        UserData = Navigation.UserData,
                        AcknowledgementData = acknowledgement,
                    };
                    _ = acknowledgementWindow.ShowDialog();
                }
            }
        }
        private bool CanAccessEdit()
        {
            return true;
        }


        private void Attach(JobOrder order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            if (order.AcknowledgmentAttachmentId == null)
            {
                AcknowledgmentAttachment attachment = new()
                {
                    JobOrderID = order.ID,
                };

                Attachment.SaveFile<AcknowledgmentAttachment>(attachment);

                order.AcknowledgmentAttachmentId = attachment.Id;
            }
            else
            {
                AcknowledgmentAttachment attachment = new()
                {
                    Id = order.AcknowledgmentAttachmentId.GetValueOrDefault(),
                };

                Attachment.UpdateFile<AcknowledgmentAttachment>(attachment);
            }

            IsLoading = false;
        }

        private bool CanAccessAttach(JobOrder order)
        {
            if (order == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DeleteAttachment(JobOrder order)
        {
            AcknowledgmentAttachment attachment = new()
            {
                Id = order.AcknowledgmentAttachmentId.GetValueOrDefault(),
            };

            Attachment.DeleteFile<AcknowledgmentAttachment>(attachment);

            order.AcknowledgmentAttachmentId = null;
        }

        private bool CanAccessDeleteAttachment(JobOrder order)
        {
            if (order == null)
                return false;

            if (order.AcknowledgmentAttachmentId == null)
                return false;

            if (!UserData.ModifyJobOrders)
                return false;

            return true;
        }

        private void DownloadAttachment(JobOrder order)
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            AcknowledgmentAttachment attachment = new()
            {
                Id = order.AcknowledgmentAttachmentId.GetValueOrDefault(),
            };

            Attachment.DownloadFile<AcknowledgmentAttachment>(attachment);

            IsLoading = false;
        }

        private bool CanAccessDownloadAttachment(JobOrder order)
        {
            if (order == null)
                return false;

            if (order.AcknowledgmentAttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment(JobOrder order)
        {
            AcknowledgmentAttachment attachment = new()
            {
                Id = order.AcknowledgmentAttachmentId.GetValueOrDefault(),
            };

            Attachment.OpenFile<AcknowledgmentAttachment>(attachment);
        }

        private bool CanAccessReadAttachment(JobOrder order)
        {
            if (order == null)
                return false;

            if (order.AcknowledgmentAttachmentId == null)
                return false;

            return true;
        }
    }
}
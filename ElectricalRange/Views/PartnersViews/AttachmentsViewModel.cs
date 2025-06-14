using ProjectsNow.Commands;
using ProjectsNow.Data;
using ProjectsNow.Data.Customers;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Suppliers;
using ProjectsNow.Data.Users;
using ProjectsNow.Printing;
using ProjectsNow.Windows.JobOrderWindows;

using Microsoft.Data.SqlClient;
using System.Windows;

namespace ProjectsNow.Views.PartnersViews
{

    internal class AttachmentsViewModel : ViewModelBase
    {
        public AttachmentsViewModel(Customer customerData, string type)
        {
            TypeData = type;
            CustomerData = customerData;

            AttachmentId = (int?)CustomerData.GetType().GetProperty($"{TypeData}AttachmentId").GetValue(CustomerData);
            AttachCommand = new RelayCommand(Attach, CanAccessAttach);
            DeleteAttachmentCommand = new RelayCommand(DeleteAttachment, CanAccessDeleteAttachment);
            DownloadAttachmentCommand = new RelayCommand(DownloadAttachment, CanAccessDownloadAttachment);
            ReadAttachmentCommand = new RelayCommand(ReadAttachment, CanAccessReadAttachment);
        }
        public AttachmentsViewModel(Supplier supplierData, string type)
        {
            TypeData = type;
            SupplierData = supplierData;

            AttachmentId = (int?)SupplierData.GetType().GetProperty($"{TypeData}AttachmentId").GetValue(SupplierData);
            AttachCommand = new RelayCommand(SupplierAttach, CanAccessSupplierAttach);
            DeleteAttachmentCommand = new RelayCommand(DeleteSupplierAttachment, CanAccessDeleteSupplierAttachment);
            DownloadAttachmentCommand = new RelayCommand(DownloadSupplierAttachment, CanAccessDownloadSupplierAttachment);
            ReadAttachmentCommand = new RelayCommand(ReadSupplierAttachment, CanAccessReadSupplierAttachment);
        }

        public int? AttachmentId { get; private set; }
        public User UserData => Navigation.UserData;
        public string TypeData { get; }
        public Customer CustomerData { get; }
        public Supplier SupplierData { get; }

        public RelayCommand AttachCommand { get; }
        public RelayCommand DeleteAttachmentCommand { get; }
        public RelayCommand DownloadAttachmentCommand { get; }
        public RelayCommand ReadAttachmentCommand { get; }


        private void Attach()
        {
            if (AttachmentId == null)
            {
                CustomerAttachment attachment = new()
                {
                    CustomerId = CustomerData.Id,
                    Type = TypeData,
                };

                Attachment.SaveFile<CustomerAttachment>(attachment);

                CustomerData.GetType().GetProperty($"{TypeData}AttachmentId").SetValue(CustomerData, attachment.Id);
            }
            else
            {
                CustomerAttachment attachment = new()
                {
                    Id = AttachmentId.Value,
                };

                Attachment.UpdateFile<CustomerAttachment>(attachment);
            }

            IsLoading = false;
        }
        private bool CanAccessAttach()
        {
            if (!UserData.ModifyCustomers)
                return false;

            return true;
        }

        private void DeleteAttachment()
        {
            CustomerAttachment attachment = new()
            {
                Id = (int)AttachmentId,
            };

            Attachment.DeleteFile<CustomerAttachment>(attachment);

            AttachmentId = null;
            CustomerData.GetType().GetProperty($"{TypeData}AttachmentId").SetValue(CustomerData, null);
        }
        private bool CanAccessDeleteAttachment()
        {
            if (AttachmentId == null)
                return false;

            if (!UserData.ModifyCustomers)
                return false;

            return true;
        }

        private void DownloadAttachment()
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            CustomerAttachment attachment = new()
            {
                Id = AttachmentId.Value,
            };

            Attachment.DownloadFile<CustomerAttachment>(attachment);

            IsLoading = false;
        }
        private bool CanAccessDownloadAttachment()
        {
            if (AttachmentId == null)
                return false;

            return true;
        }

        private void ReadAttachment()
        {
            CustomerAttachment attachment = new()
            {
                Id = AttachmentId.Value,
            };

            Attachment.OpenFile<CustomerAttachment>(attachment);
        }
        private bool CanAccessReadAttachment()
        {
            if (AttachmentId == null)
                return false;

            return true;
        }



        private void SupplierAttach()
        {
            if (AttachmentId == null)
            {
                SupplierAttachment attachment = new()
                {
                    SupplierId = SupplierData.ID,
                    Type = TypeData,
                };

                Attachment.SaveFile<SupplierAttachment>(attachment);

                SupplierData.GetType().GetProperty($"{TypeData}AttachmentId").SetValue(SupplierData, attachment.Id);
            }
            else
            {
                SupplierAttachment attachment = new()
                {
                    Id = AttachmentId.Value,
                };

                Attachment.UpdateFile<SupplierAttachment>(attachment);
            }

            IsLoading = false;
        }
        private bool CanAccessSupplierAttach()
        {
            if (!UserData.ModifySuppliers)
                return false;

            return true;
        }

        private void DeleteSupplierAttachment()
        {
            SupplierAttachment attachment = new()
            {
                Id = (int)AttachmentId,
            };

            Attachment.DeleteFile<SupplierAttachment>(attachment);

            AttachmentId = null;
            SupplierData.GetType().GetProperty($"{TypeData}AttachmentId").SetValue(SupplierData, null);
        }
        private bool CanAccessDeleteSupplierAttachment()
        {
            if (AttachmentId == null)
                return false;

            if (!UserData.ModifySuppliers)
                return false;

            return true;
        }

        private void DownloadSupplierAttachment()
        {
            IsLoading = true;
            Events.ShowEvent.Do();

            SupplierAttachment attachment = new()
            {
                Id = AttachmentId.Value,
            };

            Attachment.DownloadFile<SupplierAttachment>(attachment);

            IsLoading = false;
        }
        private bool CanAccessDownloadSupplierAttachment()
        {
            if (AttachmentId == null)
                return false;

            return true;
        }

        private void ReadSupplierAttachment()
        {
            SupplierAttachment attachment = new()
            {
                Id = AttachmentId.Value,
            };

            Attachment.OpenFile<SupplierAttachment>(attachment);
        }
        private bool CanAccessReadSupplierAttachment()
        {
            if (AttachmentId == null)
                return false;

            return true;
        }
    }
}
using Dapper.Contrib.Extensions;

using ProjectsNow.Controllers;
using ProjectsNow.Data;
using ProjectsNow.Data.JobOrders;
using ProjectsNow.Data.Users;
using ProjectsNow.Windows.MessageWindows;

using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
namespace ProjectsNow.Windows.JobOrderWindows
{

    public partial class SiteAcknowledgementWindow : Window
    {
        public User UserData { get; set; }
        public Acknowledgment AcknowledgementData { get; set; }

        public SiteAcknowledgementWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = AcknowledgementData;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Payment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 3);
        }
        private void Payment_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal value = 0;
            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                value = decimal.Parse(textBox.Text);
            }

            typeof(Acknowledgment).GetProperty(textBox.Name).SetValue(AcknowledgementData, value);

            int sum = 0;
            if (AcknowledgementData.DownPayment > 0)
            {
                sum += (int)AcknowledgementData.DownPayment;
            }

            if (AcknowledgementData.BeforeDelivery > 0)
            {
                sum += (int)AcknowledgementData.BeforeDelivery;
            }

            if (AcknowledgementData.AfterDelivery > 0)
            {
                sum += (int)AcknowledgementData.AfterDelivery;
            }

            if (AcknowledgementData.Testing > 0)
            {
                sum += (int)AcknowledgementData.Testing;
            }

            if (sum > 100)
            {
                ((TextBox)sender).Text = null;
            }
        }

        private void PaymentToggle_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            for (int i = 1; i <= 2; i++)
            {
                if (button.Name == $"PaymentToggle{i}")
                {
                    ((ToggleButton)FindName($"PaymentToggle{i}")).IsChecked = true;

                    ToggleButton toggleButton = (ToggleButton)FindName($"PaymentToggle{i}");
                    if (toggleButton.Name == "PaymentToggle1")
                    {
                        DownPayment.IsEnabled = BeforeDelivery.IsEnabled = AfterDelivery.IsEnabled = Testing.IsEnabled = true;
                        PaymentOther.IsEnabled = false;

                        AcknowledgementData.PaymentOther = null;
                    }
                    if (toggleButton.Name == "PaymentToggle2")
                    {
                        DownPayment.IsEnabled = BeforeDelivery.IsEnabled = AfterDelivery.IsEnabled = Testing.IsEnabled = false;
                        PaymentOther.IsEnabled = true;

                        AcknowledgementData.DownPayment = 70;
                        AcknowledgementData.BeforeDelivery = 30;
                        AcknowledgementData.AfterDelivery = null;
                        AcknowledgementData.Testing = null;
                    }
                }
                else
                {
                    ((ToggleButton)FindName($"PaymentToggle{i}")).Unchecked -= ToggleButton_Unchecked;
                    ((ToggleButton)FindName($"PaymentToggle{i}")).IsChecked = false;
                    ((ToggleButton)FindName($"PaymentToggle{i}")).Unchecked += ToggleButton_Unchecked;
                }
            }
        }
        private void DrawingToggle_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            for (int i = 1; i <= 5; i++)
            {
                if (button.Name == $"DrawingToggle{i}")
                {
                    ((ToggleButton)FindName($"DrawingToggle{i}")).IsChecked = true;

                    if (button.Name == "DrawingToggle1")
                    {
                        DrawingDate.IsEnabled = true;
                    }

                    if (button.Name == "DrawingToggle3")
                    {
                        DrawingPeriod.IsEnabled = DrawingUnit1.IsEnabled = DrawingCondition1.IsEnabled = true;
                    }

                    if (button.Name == "DrawingToggle4")
                    {
                        DrawingStartPeriod.IsEnabled = DrawingEndPeriod.IsEnabled = DrawingUnit2.IsEnabled = DrawingCondition2.IsEnabled = true;
                    }

                    if (button.Name == "DrawingToggle5")
                    {
                        DrawingOther.IsEnabled = true;
                    }
                }
                else
                {
                    ((ToggleButton)FindName($"DrawingToggle{i}")).Unchecked -= ToggleButton_Unchecked;
                    ((ToggleButton)FindName($"DrawingToggle{i}")).IsChecked = false;
                    ((ToggleButton)FindName($"DrawingToggle{i}")).Unchecked += ToggleButton_Unchecked;

                    ToggleButton toggleButton = (ToggleButton)FindName($"DrawingToggle{i}");
                    if (toggleButton.Name == "DrawingToggle1")
                    {
                        AcknowledgementData.DrawingDate = null;
                        DrawingDate.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DrawingToggle3")
                    {
                        AcknowledgementData.DrawingPeriod = null;
                        AcknowledgementData.DrawingUnit1 = null;
                        AcknowledgementData.DrawingCondition1 = null;

                        DrawingPeriod.IsEnabled = DrawingUnit1.IsEnabled = DrawingCondition1.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DrawingToggle4")
                    {
                        AcknowledgementData.DrawingStartPeriod = null;
                        AcknowledgementData.DrawingEndPeriod = null;
                        AcknowledgementData.DrawingUnit2 = null;
                        AcknowledgementData.DrawingCondition2 = null;

                        DrawingStartPeriod.IsEnabled = DrawingEndPeriod.IsEnabled = DrawingUnit2.IsEnabled = DrawingCondition2.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DrawingToggle5")
                    {
                        AcknowledgementData.DrawingOther = null;

                        DrawingOther.IsEnabled = false;
                    }

                    if (toggleButton.Name == "DeliveryToggle1")
                    {
                        AcknowledgementData.DeliveryDate = null;
                        DeliveryDate.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle3")
                    {
                        AcknowledgementData.DeliveryPeriod = null;
                        AcknowledgementData.DeliveryUnit1 = null;
                        AcknowledgementData.DeliveryCondition1 = null;

                        DeliveryPeriod.IsEnabled = DeliveryUnit1.IsEnabled = DeliveryCondition1.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle4")
                    {
                        AcknowledgementData.DeliveryStartPeriod = null;
                        AcknowledgementData.DeliveryEndPeriod = null;
                        AcknowledgementData.DeliveryUnit2 = null;
                        AcknowledgementData.DeliveryCondition2 = null;

                        DeliveryStartPeriod.IsEnabled = DeliveryEndPeriod.IsEnabled = DeliveryUnit2.IsEnabled = DeliveryCondition2.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle5")
                    {
                        AcknowledgementData.DeliveryOther = null;

                        DeliveryOther.IsEnabled = false;
                    }
                }
            }
        }
        private void Delivery_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            for (int i = 1; i <= 5; i++)
            {
                if (button.Name == $"DeliveryToggle{i}")
                {
                    ((ToggleButton)FindName($"DeliveryToggle{i}")).IsChecked = true;

                    if (button.Name == "DeliveryToggle1")
                    {
                        DeliveryDate.IsEnabled = true;
                    }

                    if (button.Name == "DeliveryToggle3")
                    {
                        DeliveryPeriod.IsEnabled = DeliveryUnit1.IsEnabled = DeliveryCondition1.IsEnabled = true;
                    }

                    if (button.Name == "DeliveryToggle4")
                    {
                        DeliveryStartPeriod.IsEnabled = DeliveryEndPeriod.IsEnabled = DeliveryUnit2.IsEnabled = DeliveryCondition2.IsEnabled = true;
                    }

                    if (button.Name == "DeliveryToggle5")
                    {
                        DeliveryOther.IsEnabled = true;
                    }
                }
                else
                {
                    ((ToggleButton)FindName($"DeliveryToggle{i}")).Unchecked -= ToggleButton_Unchecked;
                    ((ToggleButton)FindName($"DeliveryToggle{i}")).IsChecked = false;
                    ((ToggleButton)FindName($"DeliveryToggle{i}")).Unchecked += ToggleButton_Unchecked;

                    ToggleButton toggleButton = (ToggleButton)FindName($"DeliveryToggle{i}");
                    if (toggleButton.Name == "DeliveryToggle1")
                    {
                        AcknowledgementData.DeliveryDate = null;
                        DeliveryDate.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle3")
                    {
                        AcknowledgementData.DeliveryPeriod = null;
                        AcknowledgementData.DeliveryUnit1 = null;
                        AcknowledgementData.DeliveryCondition1 = null;

                        DeliveryPeriod.IsEnabled = DeliveryUnit1.IsEnabled = DeliveryCondition1.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle4")
                    {
                        AcknowledgementData.DeliveryStartPeriod = null;
                        AcknowledgementData.DeliveryEndPeriod = null;
                        AcknowledgementData.DeliveryUnit2 = null;
                        AcknowledgementData.DeliveryCondition2 = null;

                        DeliveryStartPeriod.IsEnabled = DeliveryEndPeriod.IsEnabled = DeliveryUnit2.IsEnabled = DeliveryCondition2.IsEnabled = false;
                    }
                    if (toggleButton.Name == "DeliveryToggle5")
                    {
                        AcknowledgementData.DeliveryOther = null;

                        DeliveryOther.IsEnabled = false;
                    }
                }
            }
        }
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int sum = 0;
            if (AcknowledgementData.DownPayment > 0)
            {
                sum += (int)AcknowledgementData.DownPayment;
            }

            if (AcknowledgementData.BeforeDelivery > 0)
            {
                sum += (int)AcknowledgementData.BeforeDelivery;
            }

            if (AcknowledgementData.AfterDelivery > 0)
            {
                sum += (int)AcknowledgementData.AfterDelivery;
            }

            if (AcknowledgementData.Testing > 0)
            {
                sum += (int)AcknowledgementData.Testing;
            }

            if (sum != 100 && PaymentToggle1.IsChecked == true)
            {
                _ = MessageWindow.Show("Payment", "Please check payment!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(PaymentOther.Text) && PaymentToggle2.IsChecked == true)
            {
                _ = MessageWindow.Show("Payment", "Please check payment!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }

            if (AcknowledgementData.DrawingDate == null && DrawingToggle1.IsChecked == true)
            {
                _ = MessageWindow.Show("Shop Drawing", "Please check shop drawing date!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if ((AcknowledgementData.DrawingPeriod == null || AcknowledgementData.DrawingUnit1 == null || AcknowledgementData.DrawingCondition1 == null) &&
                 DrawingToggle3.IsChecked == true)
            {
                _ = MessageWindow.Show("Shop Drawing", "Please check shop drawing period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if ((AcknowledgementData.DrawingStartPeriod == null || AcknowledgementData.DrawingEndPeriod == null || AcknowledgementData.DrawingUnit2 == null || AcknowledgementData.DrawingCondition2 == null) &&
                 DrawingToggle4.IsChecked == true)
            {
                _ = MessageWindow.Show("Shop Drawing", "Please check shop drawing period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if (AcknowledgementData.DrawingOther == null && DrawingToggle5.IsChecked == true)
            {
                _ = MessageWindow.Show("Shop Drawing", "Please check shop drawing period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }

            if (AcknowledgementData.DeliveryDate == null && DeliveryToggle1.IsChecked == true)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery date!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if ((AcknowledgementData.DeliveryPeriod == null || AcknowledgementData.DeliveryUnit1 == null || AcknowledgementData.DeliveryCondition1 == null) &&
                 DeliveryToggle3.IsChecked == true)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if ((AcknowledgementData.DeliveryStartPeriod == null || AcknowledgementData.DeliveryEndPeriod == null || AcknowledgementData.DeliveryUnit2 == null || AcknowledgementData.DeliveryCondition2 == null) &&
                 DeliveryToggle4.IsChecked == true)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if (AcknowledgementData.DeliveryOther == null && DeliveryToggle5.IsChecked == true)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }

            if (AcknowledgementData.DeliveryPlace == null)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery location!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }
            if (AcknowledgementData.WarrantyPeriod == null || AcknowledgementData.WarrantyUnit == null || AcknowledgementData.WarrantyCondition == null)
            {
                _ = MessageWindow.Show("Delivery", "Please check delivery period!!", MessageWindowButton.OK, MessageWindowImage.Warning);
                return;
            }

            using (SqlConnection connection = new(Database.ConnectionString))
            {
                _ = connection.Update(AcknowledgementData);
            }

            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancellation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 3);
        }
        private void Cancellation1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cancellation1.Text))
            {
                Cancellation1.Text = "0";
            }
            else
            {
                if (int.Parse(Cancellation1.Text) > 100)
                {
                    Cancellation1.Text = "0";
                }
            }
        }
        private void Cancellation2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cancellation2.Text))
            {
                Cancellation2.Text = "25";
            }
            else
            {
                if (int.Parse(Cancellation2.Text) > 100)
                {
                    Cancellation2.Text = "25";
                }
            }
        }
        private void Cancellation3_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cancellation3.Text))
            {
                Cancellation3.Text = "60";
            }
            else
            {
                if (int.Parse(Cancellation3.Text) > 100)
                {
                    Cancellation3.Text = "60";
                }
            }
        }
        private void Cancellation4_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cancellation4.Text))
            {
                Cancellation4.Text = "80";
            }
            else
            {
                if (int.Parse(Cancellation4.Text) > 100)
                {
                    Cancellation4.Text = "80";
                }
            }
        }

        private void Cancellation5_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Cancellation4.Text))
            {
                Cancellation4.Text = "100";
            }
            else
            {
                if (int.Parse(Cancellation4.Text) > 100)
                {
                    Cancellation4.Text = "100";
                }
            }
        }

        private void Period_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 2);
        }
    }
}

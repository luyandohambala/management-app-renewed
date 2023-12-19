using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace management_app_renewed
{
    public partial class Payment_Management_Page : Page, INotifyPropertyChanged
    {
        Settings settings = new Settings();

        public Payment_Management_Page()
        {
            InitializeComponent();
            DataContext = this;
            prepare_data("None");
            filter_Combobox.DataContext = new plist_model();
            filter_Combobox.SelectedValue = "None";
            currency = string.Join("", settings.load_settings().Select(filtered => filtered.Currency)).Trim();
            total_text = $"Total ({currency})";
        }

        //property used for occupant viewing
        public ObservableCollection<Payments> payment_Models { get; set; }

        public ObservableCollection<Payments> payment_count { get; set; }

        public ObservableCollection<string> Room_ComboBox_List { get; set; }

        //currently selected room type
        public string room_Type { get; set; }

        string currency { get; set; }


        public string total_text { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        //change room submit to room edit
        bool submit_to_edit = false;


        //update ui on property changes
        public void OnPropertyChanged(string propertyName1)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName1));
        }


        //prepare data for binding
        public void prepare_data(string filter)
        {
            Payments payments = new Payments();

            if (filter == "None")
            {
                payment_Models = new ObservableCollection<Payments>(payments.GetPayments());
            }
            else if (filter == "Null Balance")
            {
                payment_Models = new ObservableCollection<Payments>(payments.GetPayments().Where(filtered => Convert.ToDateTime(filtered.Date_Due) > DateTime.Today));
            }
            else if (filter == "Outstanding Balance")
            {
                payment_count = new ObservableCollection<Payments>(payments.GetPayments().Where(filtered => Convert.ToDateTime(filtered.Date_Due) < DateTime.Today));
                payment_Models = new ObservableCollection<Payments>(payments.GetPayments().Where(filtered => Convert.ToDateTime(filtered.Date_Due) < DateTime.Today));
            }
        }


        //prepare searched data
        public void search_table(string to_search)
        {
            Payments payments = new Payments();
            payment_Models = new ObservableCollection<Payments>(payments.GetPayments().Where(
                filtered => filtered.First_Name.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Last_Name.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Date_Paid.Contains(to_search.ToLower().Trim()) ||
                filtered.Date_Due.Contains(to_search.ToLower().Trim()) ||
                filtered.Room_Number.ToString().Contains(to_search.Trim())));
            filter_Combobox.SelectedValue = "None";
            OnPropertyChanged("payment_Models");
        }


        //return true if input fields are empty
        private bool validation_check()
        {
            if (String.IsNullOrEmpty(Payment_Textbox.Text) || String.IsNullOrEmpty(Date_Textbox.Text))
            {
                return true;
            }
            return false;
        }


        //clear input fields
        private void clear_items()
        {
            First_TxtBox.Text = string.Empty;
            Last_TxtBox.Text = string.Empty;
            Room_TxtBox.Text = string.Empty;
            Date_Textbox.Text = string.Empty;
            Cost_TxtBox.Text = string.Empty;
            Payment_Textbox.Text = string.Empty;
        }


        //submit room created
        private void edit_submition_method(string to_do)
        {
            if (to_do == "renew")
            {
                if ((!validation_check()) && (MessageBox.Show("Edit payment?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {      
                    Payments payments = new Payments(
                                First_TxtBox.Text.Trim(),
                                Last_TxtBox.Text.Trim(),
                                Int32.Parse(Room_TxtBox.Text.Trim()),
                                room_Type,
                                Cost_TxtBox.Text,
                                $"{currency}{Convert.ToDouble(Payment_Textbox.Text.Trim()):#,##0.00}",
                                $"{DateTime.Now:dd/MM/yyyy}",
                                Date_Textbox.Text.Trim()
                            );
                    if (payments.submit_payment("renew", payments))
                    {
                        submit_to_edit = false;
                        MessageBox.Show("Payment renewal successfull", "Management App");
                        prepare_data("None");
                        OnPropertyChanged("payment_Models");
                        clear_items();
                        View_Grid.Visibility = Visibility.Visible;
                        Edit_Label.Visibility = Visibility.Hidden;
                        Renew_Grid.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        private void edit_submit_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (submit_to_edit)
            {
                edit_submition_method("renew");
            }
        }


        //clear room_checkboxes
        private void edit_cancel_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (submit_to_edit && (MessageBox.Show("Cancel renew?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                clear_items();
                View_Grid.Visibility = Visibility.Visible;
                Edit_Label.Visibility = Visibility.Hidden;
                Renew_Grid.Visibility = Visibility.Hidden;
                submit_to_edit = false;
            }
        }


        //implement search functionality when textchanges
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearch.Text.Length > 0)
            {
                search_table(TxtSearch.Text);
            }
            else if (TxtSearch.Text.Length == 0)
            {
                prepare_data("None");
                OnPropertyChanged("payment_Models");
            }
        }


        //remove_room from table
        private void remove_payment_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Payment_DataGrid.SelectedItem != null) && (MessageBox.Show("Remove payment?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                Payments payments = (Payments)Payment_DataGrid.SelectedItem;

                if (sQLConnectionsClass.delete_info("payments", payments.First_Name.ToString(), payments.Last_Name.ToString(), 0, payments.Date_Paid))
                {
                    MessageBox.Show("Payment deletion successfull", "Management App");
                    prepare_data("None");
                    OnPropertyChanged("payment_Models");
                }
            }
            else
            {
                MessageBox.Show("Please select a payment!", "Management App");
            }
        }


        //edit room event
        private void edit_payment_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Payment_DataGrid.SelectedItem != null)
            {
                Payments payments = (Payments)Payment_DataGrid.SelectedItem;

                View_Grid.Visibility = Visibility.Hidden;
                Edit_Label.Visibility = Visibility.Visible;
                Renew_Grid.Visibility = Visibility.Visible;

                First_TxtBox.Text = payments.First_Name;
                Last_TxtBox.Text = payments.Last_Name;
                Room_TxtBox.Text = payments.Room_Number.ToString();
                Cost_TxtBox.Text = payments.Amount_Due;
                room_Type = payments.Room_Type;

                submit_to_edit = true;
            }
            else
            {
                MessageBox.Show("Please select a payment!", "Management App");
            }
        }

        private void filter_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filter_Combobox.SelectedValue.ToString().Trim() == "None")
            {
                prepare_data("None");
                OnPropertyChanged("payment_Models");
            }
            else if (filter_Combobox.SelectedValue.ToString().Trim() == "Null Balance")
            {
                prepare_data("Null Balance");
                OnPropertyChanged("payment_Models");
            }
            else if (filter_Combobox.SelectedValue.ToString().Trim() == "Outstanding Balance")
            {
                prepare_data("Outstanding Balance");
                OnPropertyChanged("payment_Models");
            }
        }
    }
    public class plist_model
    {
        //lists used for combobox binding
        public static ObservableCollection<string> filter_ComboBox_List { get; set; }

        public plist_model()
        {
            
            filter_ComboBox_List = new ObservableCollection<string>()
            {
                "None",
                "Null Balance",
                "Outstanding Balance"
            };
        }
    }
}

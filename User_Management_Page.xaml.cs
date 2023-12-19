using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace management_app_renewed
{
    public partial class User_Management_Page : Page
    {
        public User_Management_Page()
        {
            InitializeComponent();
            DataContext = this;
            prepare_data();
        }

        //property used for occupant viewing
        public ObservableCollection<New_User> user_Models { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        //change room submit to room edit
        bool submit_to_edit = false;


        //update ui on property changes
        public void OnPropertyChanged(string propertyName1)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName1));
        }


        //prepare data for binding
        public void prepare_data()
        {
            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

            user_Models = new ObservableCollection<New_User>(sQLConnectionsClass.load_user_information());
        }


        //prepare searched data
        public void search_table(string to_search)
        {
            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
            user_Models = new ObservableCollection<New_User>(sQLConnectionsClass.load_user_information().Where(
                filtered => filtered.Username.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Email.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Password.Contains(to_search.ToLower().Trim())));
            OnPropertyChanged("user_Models");
        }


        //return true if input fields are empty
        private bool validation_check()
        {
            return String.IsNullOrEmpty(username_Textbox.Text) || String.IsNullOrEmpty(Email_Textbox.Text) || String.IsNullOrEmpty(PasswordTxtBox.Password);
        }


        //clear input fields
        private void clear_items()
        {
            username_Textbox.Text = string.Empty;
            Email_Textbox.Text = string.Empty;
            PasswordTxtBox.Password = string.Empty;
        }


        //submit room created
        private void edit_submition_method(string to_do)
        {
            if (to_do == "edit")
            {
                if ((!validation_check()) && (MessageBox.Show("Edit details?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
                    var new_user_details = new New_User(username_Textbox.Text.Trim(), Email_Textbox.Text.Trim(), PasswordTxtBox.Password.Trim());
                    if (new_user_details.username_password_validation() && new_user_details.email_validation() && sQLConnectionsClass.edit_users(new_user_details))
                    {
                        submit_to_edit = false;
                        MessageBox.Show("Details successfully edited.", "Management App");
                        prepare_data();
                        OnPropertyChanged("user_Models");
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
                edit_submition_method("edit");
            }
        }


        //clear room_checkboxes
        private void edit_cancel_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (submit_to_edit && (MessageBox.Show("Cancel editing?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
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
                prepare_data();
                OnPropertyChanged("user_Models");
            }
        }


        //remove_room from table
        private void remove_user_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((User_DataGrid.SelectedItem != null) && (MessageBox.Show("Delete user?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                New_User users = (New_User)User_DataGrid.SelectedItem;

                if (sQLConnectionsClass.delete_info("users", users.Username, users.Email, 0, users.Password))
                {
                    MessageBox.Show("User deletion successfull", "Management App");
                    prepare_data();
                    OnPropertyChanged("user_Models");
                }
            }
            else
            {
                MessageBox.Show("Please select a user!", "Management App");
            }
        }


        //edit room event
        private void edit_user_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (User_DataGrid.SelectedItem != null)
            {
                New_User users = (New_User)User_DataGrid.SelectedItem;

                View_Grid.Visibility = Visibility.Hidden;
                Edit_Label.Visibility = Visibility.Visible;
                Renew_Grid.Visibility = Visibility.Visible;

                username_Textbox.Text = users.Username;
                Email_Textbox.Text = users.Email;
                PasswordTxtBox.Password = users.Password;
                

                submit_to_edit = true;
            }
            else
            {
                MessageBox.Show("Please select a user!", "Management App");
            }
        }
    }
}

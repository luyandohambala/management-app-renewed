using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace management_app_renewed
{
    public partial class Students_Management_Page : Page, INotifyPropertyChanged
    {
        Settings settings = new Settings();
        
        string currency { get; set; }
        public Students_Management_Page()
        {
            InitializeComponent();

            DataContext = this;
            slist_model list_Model = new slist_model();

            //bind comboboxes to datacontext
            Room_Combobox.DataContext = this;
            
            Gender_Combobox.DataContext = list_Model;
            currency = string.Join("", settings.load_settings().Select(filtered => filtered.Currency)).Trim();
            total_text = $"Total ({currency})";
            prepare_data("None");
            update_list_box();

        }

        //property used for occupant viewing
        public ObservableCollection<Student_Allocator> occupant_Models { get; set; }

        public ObservableCollection<int> Room_ComboBox_List { get; set; }

        //currently selected room type
        public string room_Type { get; set; }

        public string total_text { get; set; }

        //cost of currently selected room
        public string room_Cost { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;


        //change room submit to room edit
        bool submit_to_edit = false;


        //update ui on property changes
        public void OnPropertyChanged(string propertyName1, string propertyName2)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName1));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName2));
        }


        //property used for total number of occupants
        public string occupant_numbers { get; set; }


        //prepare data for binding
        public void prepare_data(string filter)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();

            if (filter == "None")
            {
                occupant_Models = new ObservableCollection<Student_Allocator>(SQL_Connection.Load_Occupant_Information());
                occupant_numbers = $"View Occupants: {occupant_Models.Count}";
            }
        }


        //update room_list_box
        public void update_list_box()
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
            Room_ComboBox_List = new ObservableCollection<int>(SQL_Connection.Load_Room_Information().
                        Where(filtered => (filtered.Room_Type.ToLower() == "single shared" && filtered.Occupants == 0) ||
                        (filtered.Room_Type.ToLower() == "double shared" && filtered.Occupants < 2) ||
                        (filtered.Room_Type.ToLower() == "four shared" && filtered.Occupants < 4)).Select(filtered => filtered.Room_Number));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Room_ComboBox_List"));

        }


        //prepare searched data
        public void search_table(string to_search)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
            occupant_Models = new ObservableCollection<Student_Allocator>(SQL_Connection.Load_Occupant_Information().Where(
                filtered => filtered.First_Name.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Last_Name.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Gender.ToLower().Contains(to_search.ToLower().Trim()) ||
                filtered.Room_Number.ToString().Contains(to_search.Trim())));

            occupant_numbers = $"View Occupants: {occupant_Models.Count}";
            OnPropertyChanged("occupant_Models", "occupant_numbers");
        }


        //return true if input fields are empty
        private bool validation_check()
        {
            if (String.IsNullOrEmpty(First_Textbox.Text) || String.IsNullOrEmpty(Last_Textbox.Text) || Room_Combobox.SelectedValue == null ||
                Gender_Combobox.SelectedValue == null || String.IsNullOrEmpty(Payment_Textbox.Text) || String.IsNullOrEmpty(Date_Textbox.Text))
            {
                return true;
            }
            return false;
        }


        //clear input fields
        private void clear_items()
        {
            First_Textbox.Clear();
            Last_Textbox.Clear();
            Payment_Textbox.Clear();
            Room_Combobox.SelectedItem = null;
            Gender_Combobox.SelectedItem = null;
        }


        //submit room created
        private void occupant_submition_method(string to_do)
        {
            if (to_do == "submit")
            {
                if (!validation_check())
                {
                    try
                    {
                        if (MessageBox.Show("Add Occupant?", "Management Application", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Student_Allocator student_allocation = new Student_Allocator(
                                First_Textbox.Text.Trim(),
                                Last_Textbox.Text.Trim(),
                                Gender_Combobox.SelectedValue.ToString().Trim(),
                                Int32.Parse(Room_Combobox.SelectedValue.ToString().Trim())
                            );

                            Payments payments = new Payments(
                                First_Textbox.Text.Trim(),
                                Last_Textbox.Text.Trim(),
                                Int32.Parse(Room_Combobox.SelectedValue.ToString()),
                                $"{string.Join("", new SQLConnectionsClass().Load_Room_Information().Where(filtered => filtered.Room_Number.ToString() == Room_Combobox.SelectedValue.ToString()).Select(filtered => filtered.Room_Type))}",
                                $"{string.Join("", new SQLConnectionsClass().Load_Room_Information().Where(filtered => filtered.Room_Number.ToString() == Room_Combobox.SelectedValue.ToString()).Select(filtered => filtered.Cost)):#,##0.00}",
                                $"{currency}{Convert.ToDouble(Payment_Textbox.Text.Trim()):#,##0.00}",
                                $"{DateTime.Now:dd/MM/yyyy}",
                                Date_Textbox.Text.Trim()
                            ); ;

                            if (student_allocation.create_student(student_allocation) && payments.submit_payment("submit", payments))
                            {
                                prepare_data("None");
                                MessageBox.Show("Occupant Successfully added", "Management Application");
                                OnPropertyChanged("occupant_Models", "occupant_numbers");
                                update_list_box();
                                clear_items();
                            }
                        }
                    }
                    catch (SQLiteException SFE)
                    {
                        MessageBox.Show($"Error code: {SFE.Message}\r\nAppropriately fill in all values.", "Management Application");
                        clear_items();
                    }
                }
                else
                {
                    MessageBox.Show("Appropriately fill in all values.", "Management Application");
                }
            }
            
            else if (to_do == "edit")
            {
                if ((!validation_check()) && (MessageBox.Show("Edit student?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    slist_model slist_Model = new slist_model();
                    
                    SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                    Student_Allocator student_allocation = new Student_Allocator(
                            First_Textbox.Text.Trim(),
                            Last_Textbox.Text.Trim(),
                            Gender_Combobox.SelectedValue.ToString().Trim(),
                            Int32.Parse(Room_Combobox.SelectedValue.ToString().Trim())
                        );

                    if (sQLConnectionsClass.Edit_Occupant_Information(student_allocation))
                    {
                        submit_to_edit = false;
                        MessageBox.Show("Student edit successfull", "Management App");
                        prepare_data("None");
                        OnPropertyChanged("occupant_Models", "occupant_numbers");
                        update_list_box();
                        clear_items();
                    }
                }
            }
        }
        private void Occupant_Submit_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!submit_to_edit)
            {
                occupant_submition_method("submit");
            }
            else
            {
                occupant_submition_method("edit");
            }

        }


        //clear room_checkboxes
        private void Occupant_Clear_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (submit_to_edit && (MessageBox.Show("Cancel edit?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                clear_items();
                submit_to_edit = false;
            }
            else
            {
                clear_items();
            }
        }


        //navigate view on rooms
        public void navigate_rooms(string view)
        {
            if (view == "add")
            {
                Create_Grid.Visibility = Visibility.Visible;
                View_Grid.Visibility = Visibility.Hidden;
            }
            else if (view == "view")
            {
                Create_Grid.Visibility = Visibility.Hidden;
                View_Grid.Visibility = Visibility.Visible;
                OnPropertyChanged("occupant_Models", "occupant_numbers");
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
                OnPropertyChanged("occupant_Models", "occupant_numbers");
            }
        }


        //create rooms for datagridview
        private void add_occupant_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            navigate_rooms("add");
        }


        //view rooms in datagridview
        private void view_occupant_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            navigate_rooms("view");
        }


        //remove_room from table
        private void remove_occupant_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Occupants_DataGrid.SelectedItem != null) && (MessageBox.Show("Remove Occupant?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                Student_Allocator student_Allocator = (Student_Allocator)Occupants_DataGrid.SelectedItem;

                if (sQLConnectionsClass.delete_info("students", student_Allocator.First_Name.ToString(), student_Allocator.Last_Name.ToString(), student_Allocator.Room_Number, null))
                {
                    MessageBox.Show("Occupant deletion successfull", "Management App");
                    prepare_data("None");
                    OnPropertyChanged("occupant_Models", "occupant_numbers");
                    update_list_box();
                }
            }
            else
            {
                MessageBox.Show("Please select an Occupant first!", "Management App");
            }
        }


        //edit room event
        private void edit_occupant_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Occupants_DataGrid.SelectedItem != null)
            {
                Student_Allocator student_Allocator = (Student_Allocator)Occupants_DataGrid.SelectedItem;

                navigate_rooms("add");

                First_Textbox.Text = student_Allocator.First_Name.ToString();
                Last_Textbox.Text = student_Allocator.Last_Name.ToString();
                Room_Combobox.SelectedValue = student_Allocator.Room_Number.ToString();
                Gender_Combobox.SelectedValue = student_Allocator.Gender.ToString();

                submit_to_edit = true;
            }
            else
            {
                MessageBox.Show("Please select a student first!", "Management App");
            }
        }

        private void Room_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();         
            
            if (Room_Combobox.SelectedValue == null)
            {
                room_Type = string.Empty;
                room_Cost = "K0.00";
                OnPropertyChanged("room_Type", "room_Cost");
            }
            else
            {
                room_Type = string.Join(" ", SQL_Connection.Load_Room_Information().Where(filtered => filtered.Room_Number.ToString() == Room_Combobox.SelectedValue.ToString())
                                                                          .Select(filtered => filtered.Room_Type));
                room_Cost = $"K{string.Join(" ", SQL_Connection.Load_Room_Information().Where(filtered => filtered.Room_Number.ToString() == Room_Combobox.SelectedValue.ToString()).Select(filtered => filtered.Cost))}";
                
                OnPropertyChanged("room_Type", null);
            }

            
        }
    }

    public class slist_model
    {
        //lists used for combobox binding
        public static ObservableCollection<string> Gender_ComboBox_List { get; set; }
        
        public slist_model()
        {
            Gender_ComboBox_List = new ObservableCollection<string>(){
                "Male",
                "Female"
            };

        }

    }
}

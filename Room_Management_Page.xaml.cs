using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace management_app_renewed
{
    
    public partial class Room_Management_Page : Page, INotifyPropertyChanged
    {
        Settings settings = new Settings();

        public Room_Management_Page()
        {
            InitializeComponent();
            DataContext = this;
            list_model list_Model = new list_model();
            //bind comboboxes to datacontext
            Room_Combobox.SelectedIndex = 0;
            Floor_Combobox.DataContext = list_Model;
            Gender_Combobox.DataContext = list_Model;
            filter_Combobox.DataContext = list_Model;

            currency = string.Join("", settings.load_settings().Select(filtered => filtered.Currency)).Trim();
            first = string.Join("", settings.load_settings().Select(filtered => filtered.HTsingle_amount)).Trim();
            second = string.Join("", settings.load_settings().Select(filtered => filtered.HTdouble_amount)).Trim();
            third = string.Join("", settings.load_settings().Select(filtered => filtered.HBfour_amount)).Trim();
            check_type();

            filter_Combobox.SelectedValue = "None";
            prepare_data("None");
        }

        string currency { get; set; }

        string first { get; set; }
        string second { get; set; }
        string third { get; set; }
        //property used for room viewing
        public ObservableCollection<Room_Allocator> room_Models { get; set; }

        
        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility Gender_Visibility { get; set; }

        public void check_type()
        {
            Settings settings = new Settings();

            var filtered_list = string.Join("", settings.load_settings().Select(filtered => filtered.Room_Allocation)).Trim();

            if (filtered_list != null)
            {
                if (filtered_list == "Hotel/Lodge")
                {
                    Gender_Visibility = Visibility.Collapsed;
                    gender_col.Visibility = Visibility.Collapsed;
                    OnPropertyChanged("Gender_Visibility", "settings.Room_Allocation");
                    Room_Combobox.ItemsSource = new string[] { "Single", "Double" };
                    Gender_Combobox.ItemsSource = new string[] { "Not set" };
                    Gender_Combobox.SelectedIndex = 0;
                    htactive = false;
                }
                else if (filtered_list == "Hostel/Boarding House")
                {
                    Gender_Visibility = Visibility.Visible;
                    gender_col.Visibility = Visibility.Visible;
                    OnPropertyChanged("Gender_Visibility", "settings.Room_Allocation");
                    Room_Combobox.ItemsSource = new string[] { "Single Shared", "Double Shared", "Four Shared" };
                    Gender_Combobox.DataContext = new list_model();
                    htactive = true;
                }
            }
            else
            {
                Gender_Visibility = Visibility.Collapsed;
                OnPropertyChanged("Gender_Visibility", "settings.Room_Allocation");
                Room_Combobox.ItemsSource = new string[] { "Single", "Double" };
            }

            filtered_list = string.Join("", settings.load_settings().Select(filtered => filtered.Floor_Number)).Trim();

            if (filtered_list == "0")
            {
                Floor_Combobox.ItemsSource = new string[] { "Ground floor" };
            }
            else if (Int32.Parse(filtered_list) > 0)
            {
                string[] new_collection = new string[Int32.Parse(filtered_list) + 1];
                new_collection[0] = "Ground floor";
                
                for (int i = 1; i <= Int32.Parse(filtered_list); i++)
                {
                    new_collection[i] = $"{i} floor";
                }

                Floor_Combobox.ItemsSource = new_collection;
            }
        }

        
        //change room submit to room edit
        bool submit_to_edit = false;


        bool htactive = true;

        
        //update ui on property changes
        public void OnPropertyChanged(string propertyName1, string propertyName2)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName1));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName2));
        }


        //property used for total number of rooms
        public string room_numbers { get; set; }


        //prepare data for binding
        public void prepare_data(string filter)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
                        
            if (filter == "None")
            {
                room_Models = new ObservableCollection<Room_Allocator>(SQL_Connection.Load_Room_Information());
                room_numbers = $"View Rooms: {room_Models.Count}";
                filter_Combobox.SelectedValue = "None";
            }
            else if (filter == "Rooms Available")
            {
                    room_Models = new ObservableCollection<Room_Allocator>(SQL_Connection.Load_Room_Information().
                        Where(filtered => (filtered.Room_Type.ToLower() == "single shared" && filtered.Occupants == 0) ||
                        (filtered.Room_Type.ToLower() == "double shared" && filtered.Occupants < 2) ||
                        (filtered.Room_Type.ToLower() == "four shared" && filtered.Occupants < 4)));
                    room_numbers = $"View Rooms: {room_Models.Count}";                
            }
            else if (filter == "Rooms Occupied")
            {
                room_Models = new ObservableCollection<Room_Allocator>(SQL_Connection.Load_Room_Information().
                        Where(filtered => (filtered.Room_Type.ToLower() == "single shared" && filtered.Occupants == 1) ||
                        (filtered.Room_Type.ToLower() == "double shared" && filtered.Occupants == 2) ||
                        (filtered.Room_Type.ToLower() == "four shared" && filtered.Occupants == 4)));
                room_numbers = $"View Rooms: {room_Models.Count}";
            }
        }

        
        //prepare searched data
        public void search_table(string to_search)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
            room_Models = new ObservableCollection<Room_Allocator>(SQL_Connection.Load_Room_Information().Where(
                filtered => filtered.Room_Type.ToLower().Contains(to_search.ToLower().Trim()) || 
                filtered.Room_Number.ToString().Contains(to_search.Trim()) ||
                filtered.Gender_Type.ToLower().Contains(to_search.ToLower().Trim())));

            room_numbers = $"View Rooms: {room_Models.Count}";
            filter_Combobox.SelectedValue = "None";
            OnPropertyChanged("room_Models", "room_numbers");
        }

        
        //return true if input fields are empty
        private bool validation_check(bool active)
        {
            if (active)
            {
                if (String.IsNullOrEmpty(Room_Textbox.Text) || Room_Combobox.SelectedValue == null ||
                Floor_Combobox.SelectedValue == null || Gender_Combobox.SelectedValue == null)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (String.IsNullOrEmpty(Room_Textbox.Text) || Room_Combobox.SelectedValue == null ||
                Floor_Combobox.SelectedValue == null)
                {
                    return true;
                }
                return false;
            }
            
        }

        
        //clear input fields
        private void clear_items()
        {
            Room_Textbox.Clear();

            Room_Combobox.SelectedItem = null;
            Floor_Combobox.SelectedItem = null;
            Gender_Combobox.SelectedItem = null;
        }

        
        //submit room created
        private void room_submition_method(string to_do)
        {
            if (to_do == "submit")
            {
                if (!validation_check(htactive))
                {
                    try
                    {
                        if (MessageBox.Show("Add room?", "Management Application", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Room_Allocator room_allocation = new Room_Allocator(
                                Int32.Parse(Room_Textbox.Text.Trim()),
                                Room_Combobox.SelectedValue.ToString().Trim(),
                                Floor_Combobox.SelectedValue.ToString().Trim(),
                                0,
                                Gender_Combobox.SelectedValue.ToString().Trim(),
                                Cost_TxtBox.Text.Trim()
                            );

                            if (room_allocation.create_room("submit", room_allocation))
                            {
                                prepare_data("None");
                                MessageBox.Show("Room Successfully created", "Management Application");
                                OnPropertyChanged("room_Models", "room_numbers");
                                clear_items();
                            }
                        }
                    }
                    catch (FormatException SFE)
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
                if ((!validation_check(htactive)) && (MessageBox.Show("Edit room?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                    Room_Allocator room_allocation = new Room_Allocator(
                            Int32.Parse(Room_Textbox.Text.Trim()),
                            Room_Combobox.SelectedValue.ToString().Trim(),
                            Floor_Combobox.SelectedValue.ToString().Trim(),
                            0,
                            Gender_Combobox.SelectedValue.ToString().Trim(),
                            Cost_TxtBox.Text.Trim()
                            );

                    if (sQLConnectionsClass.Register_Room_Information("edit", room_allocation))
                    {
                        submit_to_edit = false;
                        MessageBox.Show("Room edit successfull", "Management App");
                        prepare_data("None");
                        OnPropertyChanged("room_Models", "room_numbers");
                        clear_items();
                    }
                }
            }
        }
        private void Room_Submit_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!submit_to_edit)
            {
                room_submition_method("submit");
            }
            else
            {
                room_submition_method("edit");
            }
            
        }

        
        //clear room_checkboxes
        private void Room_Clear_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
            if (view == "create")
            {
                Create_Grid.Visibility = Visibility.Visible;
                View_Grid.Visibility = Visibility.Hidden;
            }
            else if (view == "view")
            {
                Create_Grid.Visibility = Visibility.Hidden;
                View_Grid.Visibility = Visibility.Visible;
                OnPropertyChanged("room_Models", "room_numbers");
            }
        }


        //implement search functionality when textchanges
        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TxtSearch.Text.Length > 0)
            {
                search_table(TxtSearch.Text);
            }
            else if (TxtSearch.Text.Length == 0)
            {
                prepare_data("None");
                OnPropertyChanged("room_Models", "room_numbers");
            }
        }


        //create rooms for datagridview
        private void create_rooms_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            navigate_rooms("create");
        }

        
        //view rooms in datagridview
        private void view_rooms_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            navigate_rooms("view");
        }


        //remove_room from table
        private void remove_room_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Rooms_DataGrid.SelectedItem != null) && (MessageBox.Show("Remove room?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
                SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

                Room_Allocator room_Allocator = (Room_Allocator)Rooms_DataGrid.SelectedItem;

                var counter = string.Join(" ", sQLConnectionsClass.Load_Room_Information().Where(filtered => filtered.Room_Number == room_Allocator.Room_Number)
                                                                          .Select(filtered => filtered.Occupants));

                if ((Int32.Parse(counter.Trim()) == 0) && sQLConnectionsClass.delete_info("rooms", room_Allocator.Room_Number.ToString(), null, 0, null))
                {
                    MessageBox.Show("Room deletion successfull", "Management App");
                    prepare_data("None");
                    OnPropertyChanged("room_Models", "room_numbers");
                }
                else
                {
                    MessageBox.Show("Cannot delete a room with occupants!", "Management App");
                }
            }
            else
            {
                MessageBox.Show("Please select a room first!", "Management App");
            }
        }

        
        //edit room event
        private void edit_room_field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Rooms_DataGrid.SelectedItem != null)
            {
                Room_Allocator room_Allocator = (Room_Allocator)Rooms_DataGrid.SelectedItem;

                navigate_rooms("create");

                Room_Textbox.Text = room_Allocator.Room_Number.ToString();
                Room_Combobox.SelectedValue = room_Allocator.Room_Type.ToString();
                Floor_Combobox.SelectedValue = room_Allocator.Floor_Number.ToString();
                Gender_Combobox.SelectedValue = room_Allocator.Gender_Type.ToString();

                submit_to_edit = true;
            }
            else
            {
                MessageBox.Show("Please select a room first!", "Management App");
            }
        }

        
        //filter combobox event
        private void filter_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filter_Combobox.SelectedValue.ToString().Trim() == "None")
            {
                prepare_data("None");
                OnPropertyChanged("room_Models", "room_numbers");
            }
            else if (filter_Combobox.SelectedValue.ToString().Trim() == "Rooms Available")
            {
                prepare_data("Rooms Available");
                OnPropertyChanged("room_Models", "room_numbers");
            }
            else if (filter_Combobox.SelectedValue.ToString().Trim() == "Rooms Occupied")
            {
                prepare_data("Rooms Occupied");
                OnPropertyChanged("room_Models", "room_numbers");
            }
        }

        private void Room_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Room_Combobox.SelectedValue != null)
            {

                if (Room_Combobox.SelectedValue.ToString().ToLower().Contains("single"))
                {
                    Cost_TxtBox.Text = $"{currency}{first:#,##0.00}";
                }
                else if (Room_Combobox.SelectedValue.ToString().ToLower().Contains("double"))
                {
                    Cost_TxtBox.Text = $"{currency}{second:#,##0.00}";
                }
                else if (Room_Combobox.SelectedValue.ToString().ToLower().Contains("four"))
                {
                    Cost_TxtBox.Text = $"{currency}{third:#,##0.00}";
                }
            }
            else
            {
                Cost_TxtBox.Text = string.Empty;
            }
        }
    }

    public class list_model
    {
        //lists used for combobox binding
        public static ObservableCollection<string> Gender_ComboBox_List { get; set; }
        public static ObservableCollection<string> filter_ComboBox_List { get; set; }

        public list_model()
        {
            
            Gender_ComboBox_List = new ObservableCollection<string>(){
                "Male",
                "Female"
            };
            

            filter_ComboBox_List = new ObservableCollection<string>()
            {
                "None",
                "Rooms Available",
                "Rooms Occupied"
            };
        }
        
    }
}

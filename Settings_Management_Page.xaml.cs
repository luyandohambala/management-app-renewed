using ByteSizeLib;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace management_app_renewed
{

    public partial class Settings_Management_Page : Page, INotifyPropertyChanged
    {

        //room allocation property
        public string room_description { get; set; }

        public ObservableCollection<string> RoomAlloc_ComboBox_List { get; set; }

        public string Company_Name { get; set; }
        public byte[]? Company_Logo { get; set; }
        
        public BitmapImage? Company_Image { get; set; }
        
        public int Floor_Number { get; set; }
        public string Room_Allocation { get; set; }
        
        public char Currency { get; set; }
        
        public string HTsingle_amount { get; set; }
        public string HTdouble_amount { get; set; }
        public string HBfour_amount { get; set; }



        bool hotel_boarding = true;
        
        public Settings_Management_Page()
        {
            InitializeComponent();

            load_data();
            load_image();

            DataContext = this;

            RoomAlloc_ComboBox_List = new ObservableCollection<string>() { "Hotel/Lodge", "Hostel/Boarding House" };
            RoomAlloc_Combobox.ItemsSource = RoomAlloc_ComboBox_List;
            RoomAlloc_Combobox.SelectedItem = Room_Allocation;
        }


        private void clear_fields(bool active)
        {
            if (active)
            {
                SingleN_TxtBox.Text = "0.00";
                DoubleN_TxtBox.Text = "0.00";
            }
            else
            {
                BSingleN_TxtBox.Text = "0.00";
                BDoubleN_TxtBox.Text = "0.00";
                BFourN_TxtBox.Text = "0.00";
            }
        }


        private void load_data()
        {
            var list_to_filter = new Settings().load_settings();

            Company_Name = string.Join("", list_to_filter.Select(filtered => filtered.Company_Name)).Trim();
            Floor_Number = Int32.Parse(string.Join("", list_to_filter.Select(filtered => filtered.Floor_Number)).Trim());
            Room_Allocation = string.Join("", list_to_filter.Select(filtered => filtered.Room_Allocation)).Trim();
            Currency = Convert.ToChar(string.Join("", list_to_filter.Select(filtered => filtered.Currency)).Trim());
            HTsingle_amount = string.Join("", list_to_filter.Select(filtered => filtered.HTsingle_amount)).Trim();
            HTdouble_amount = string.Join("", list_to_filter.Select(filtered => filtered.HTdouble_amount)).Trim();
            HBfour_amount = string.Join("", list_to_filter.Select(filtered => filtered.HBfour_amount)).Trim();
        }

        public void load_image()
        {
            var list_to_filter1 = new SQLConnectionsClass().load_image();
            Company_Logo = list_to_filter1;

            if (Company_Logo != null)
            {
                Company_Image = convert_image(null);
            }
        }

        private bool validation_check(bool active)
        {
            if (active)
            {
                if (String.IsNullOrEmpty(CompanyN_TxtBox.Text) || RoomAlloc_Combobox.SelectedValue is null || String.IsNullOrEmpty(CurrencyN_TxtBox.Text) ||
                    String.IsNullOrEmpty(FloorN_TxtBox.Text) || String.IsNullOrEmpty(SingleN_TxtBox.Text) || String.IsNullOrEmpty(DoubleN_TxtBox.Text))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(CompanyN_TxtBox.Text) || RoomAlloc_Combobox.SelectedValue is null || String.IsNullOrEmpty(FloorN_TxtBox.Text) ||
                    String.IsNullOrEmpty(CurrencyN_TxtBox.Text) || String.IsNullOrEmpty(BSingleN_TxtBox.Text) || String.IsNullOrEmpty(BDoubleN_TxtBox.Text) || String.IsNullOrEmpty(BFourN_TxtBox.Text))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RoomAlloc_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomAlloc_Combobox.SelectedValue != null)
            {
                if (RoomAlloc_Combobox.SelectedValue.ToString() == "Hotel/Lodge")
                {
                    room_description = "Single, Double";

                    hotel_management_code.IsEnabled = true;
                    hotel_management_code.Background = (Brush)new BrushConverter().ConvertFrom("White");
                    boarding_management_code.IsEnabled = false;
                    boarding_management_code.Background = (Brush)new BrushConverter().ConvertFrom("LightGray");
                    clear_fields(false);
                    hotel_boarding = true;
                }
                else if (RoomAlloc_Combobox.SelectedValue.ToString() == "Hostel/Boarding House")
                {

                    room_description = "Single, Double, Four";
                    
                    hotel_management_code.IsEnabled = false;
                    hotel_management_code.Background = (Brush)new BrushConverter().ConvertFrom("LightGray");
                    boarding_management_code.IsEnabled = true;
                    boarding_management_code.Background = (Brush)new BrushConverter().ConvertFrom("White");
                    clear_fields(true);
                    hotel_boarding = false;
                }
                OnPropertyChanged("room_description");
            }
        }

        private void submit_btn_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (validation_check(hotel_boarding) && 
                MessageBox.Show("If room structure date has been changed all room and occupant information will be erased! Save changes?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (hotel_boarding)
                {
                    Settings settings = new Settings(
                        CompanyN_TxtBox.Text,
                        Company_Logo,
                        Int32.Parse(FloorN_TxtBox.Text),
                        RoomAlloc_Combobox.SelectedValue.ToString(),
                        Convert.ToChar(CurrencyN_TxtBox.Text),
                        $"{Convert.ToDouble(SingleN_TxtBox.Text.Trim()):#,##0.00}",
                        $"{Convert.ToDouble(DoubleN_TxtBox.Text.Trim()):#,##0.00}",
                        "0.00"
                    );
                    settings.save_settings(settings);
                    load_data();
                    SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
                    if (sQLConnectionsClass.delete_info("delete all", null, null, 0, null))
                    {
                        //MessageBox.Show("Occupant and Room information reset.", "Mangement App");
                    }
                }
                else
                {
                    Settings settings = new Settings(
                        CompanyN_TxtBox.Text,
                        Company_Logo,
                        Int32.Parse(FloorN_TxtBox.Text),
                        RoomAlloc_Combobox.SelectedValue.ToString(),
                        Convert.ToChar(CurrencyN_TxtBox.Text),
                        $"{Convert.ToDouble(BSingleN_TxtBox.Text.Trim()):#,##0.00}",
                        $"{Convert.ToDouble(BDoubleN_TxtBox.Text.Trim()):#,##0.00}",
                        $"{Convert.ToDouble(BFourN_TxtBox.Text.Trim()):#,##0.00}"
                    );
                    settings.save_settings(settings);
                    load_data();
                    SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
                    if (sQLConnectionsClass.delete_info("delete all", null, null, 0, null))
                    {
                        MessageBox.Show("Occupant and Room information reset.", "Mangement App");
                    }
                }
                MessageBox.Show("Changes saved.", "Management App");
            }
        }

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Reset to default?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Settings settings = new Settings(
                        "Not set",
                        null,
                        0,
                        "Hotel/Lodge",
                        'K',
                        "0.00",
                        "0.00",
                        "0.00"
                        );
                
                if (settings.save_settings(settings))
                {
                    MessageBox.Show("Default set.", "Management App");
                }
            }
        }

        private BitmapImage convert_image(string? filename)
        {
            if (filename != null)
            {
                byte[] bytes;
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                }

                var image_memory_stream = new MemoryStream(bytes);

                Company_Logo = bytes;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = image_memory_stream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
            else
            {
                BitmapImage img = new BitmapImage();
                using (MemoryStream memStream = new MemoryStream(Company_Logo))
                {
                    img.BeginInit();
                    img.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = memStream;
                    img.EndInit();
                }
                return img;
            }
        }
        
        private void Select_Logo_btn_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                
                if (new FileInfo(openFileDialog.FileName).Length > 2000000)
                {
                    MessageBox.Show("Selected image size not supported", "Management App");
                    openFileDialog.ShowDialog();
                }
                else
                {
                    Company_Image = convert_image(openFileDialog.FileName);

                    OnPropertyChanged("Company_Image");
                }
            }
        }

        private void Clear_Logo_btn_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Clear logo?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Company_Image = null;
                Company_Logo = null;
                OnPropertyChanged("Company_Image");
            }
        }
    }

    public class selist_model
    {
        //lists used for combobox binding
        public static ObservableCollection<string>? RoomAlloc_ComboBox_List { get; set; }

        public selist_model()
        {

            RoomAlloc_ComboBox_List = new ObservableCollection<string>()
            {
                "Hotel/Lodge",
                "Hostel/Boarding House"
            };
        }
    }
}

using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace management_app_renewed
{
    public partial class DashboardWindow : Window
    {

        // used to set navigation icon
        bool IsNavigation = false;

        public string username_string { get; set; }

        public DashboardWindow()
        {
            
            InitializeComponent();
            
            DataContext = this;
            Settings_Management_Page settings_Management_Page = new Settings_Management_Page();
            Logo_Image.DataContext = settings_Management_Page;
            settings_Management_Page.load_image();
            
        }

        //logs out current user
        private void Logout_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Logout?", "Management App", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Close();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        
        //allows dragability of window
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


        //navigation colors
        private void change_nav_color(string nav)
        {
            BrushConverter color = new BrushConverter();

            if (nav == "home")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                Home_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");
                
                Room_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Room_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Student_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
                
                Payment_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                User_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                User_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
                
                Settings_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
            }

            else if (nav == "room")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Home_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Room_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                Room_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Student_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Payment_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                User_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                User_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Settings_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
            }

            else if (nav == "student")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Home_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Room_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Room_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                Student_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");

                Payment_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                User_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                User_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Settings_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
            }

            else if (nav == "payment")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Home_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Room_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Room_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Student_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Payment_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");

                User_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                User_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Settings_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("LightGray");
            }

            else if (nav == "users")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Home_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Room_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Room_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Student_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Payment_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                User_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                User_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");

                Settings_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

            }

            else if (nav == "settings")
            {
                Home_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Home_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Room_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Room_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Student_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Student_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Payment_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                Payment_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                User_Logo.Foreground = (Brush)color.ConvertFrom("LightGray");
                User_Text.Foreground = (Brush)color.ConvertFrom("LightGray");

                Settings_Logo.Foreground = (Brush)color.ConvertFrom("#ffffff");
                Settings_Text.Foreground = (Brush)color.ConvertFrom("#ffffff");
            }
        }
        

        //display_Window mechanic
        public void Display_Window_Mechanic(int passed_variable)
        {
            if (passed_variable == 1)
            {
                //set display frame to corresponding page and change background navigation buttons                
                change_nav_color("home");
                Display_Window.Content = new Home_Page();
            }
            else if (passed_variable == 2)
            {
                //set display frame to corresponding page and change background navigation buttons
                change_nav_color("room");
                Display_Window.Content = new Room_Management_Page();
            }
            else if (passed_variable == 3)
            {
                //set display frame to corresponding page and change background navigation buttons
                change_nav_color("student");
                Display_Window.Content = new Students_Management_Page();
            }
            else if (passed_variable == 4)
            {
                //set display frame to corresponding page and change background navigation buttons
                change_nav_color("payment");
                Display_Window.Content = new Payment_Management_Page();
            }

            else if (passed_variable == 5)
            {
                change_nav_color("users");
                Display_Window.Content = new User_Management_Page();
            }
            
            else if (passed_variable == 6)
            {
                //set display frame to corresponding page and change background navigation buttons
                change_nav_color("settings");
                Display_Window.Content = new Settings_Management_Page();
            }

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Display_Window_Mechanic(1);
        }

        //display dashboard content
        private void Home_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(1);
        }

        private void Room_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(2);
        }

        private void Student_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(3);
        }

        private void Payment_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(4);
        }

        private void Settings_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(6);
        }

        private void User_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Display_Window_Mechanic(5);
        }
    }

    //class below updates time allocator textblock
    public class Ticker : INotifyPropertyChanged
    {
        public Ticker()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 1000 // 1 second updates
            };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Now)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

    }
}

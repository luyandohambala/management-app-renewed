using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace management_app_renewed
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //sign in verification
        bool sign_in = true;
        bool forget_password = false;
        SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
        Existing_User user_details;
        New_User new_user_details;
        DispatcherTimer dt = new DispatcherTimer();

        public string timer_interval { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Dt_Tick;
            using (IDbConnection db = new SQLiteConnection(SQL_Connection.Connect()))
            {
                /*db.Execute("PRAGMA foreign_keys = ON;");
                //using database connection create tables with database below
                db.Execute(@"CREATE TABLE IF NOT EXISTS Users (
	                        Username TEXT,
	                        Email TEXT UNIQUE,
	                        Password TEXT NOT NULL,
	                        PRIMARY KEY(Username)
                            );");

                //create rooms table
                db.Execute(@"CREATE TABLE IF NOT EXISTS Rooms (
	                        Room_Number INTEGER PRIMARY KEY,
	                        Room_Type TEXT UNIQUE,
	                        Floor_Number TEXT NOT NULL,
	                        Occupants INTEGER NOT NULL,
	                        Gender_Type TEXT NOT NULL
                            );");*/

                /*//create occupants (students) table
                db.Execute(@"CREATE TABLE IF NOT EXISTS Occupants (
	                        First_Name TEXT NOT NULL,
	                        Last_Name TEXT NOT NULL,
                            Gender TEXT NOT NULL,
	                        Room_Number	INTEGER NOT NULL
	                        FOREIGN KEY(Room_Number) REFERENCES (Rooms.Room_Number)
                            )");*/

            }
        }

        //username textbox event
        private void EmailText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailTxtBox.Focus();
        }

        private void EmailTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTxtBox.Text) && EmailTxtBox.Text.Length > 0)
            {
                EmailText.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmailText.Visibility = Visibility.Visible;
            }
        }


        //email textbox event
        private void EmailDetailTxtBox_S_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailDetailTxtBox_S.Text) && EmailDetailTxtBox_S.Text.Length > 0)
            {
                EmailDetailText_S.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmailDetailText_S.Visibility = Visibility.Visible;
            }
        }

        private void EmailDetailText_S_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailDetailTxtBox_S.Focus();
        }


        //password textbox event
        private void PasswordTxtBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordTxtBox.Password) && PasswordTxtBox.Password.Length > 0)
            {
                PasswordText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordText.Visibility = Visibility.Visible;
            }
        }

        private void PasswordText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordTxtBox.Focus();
        }


        //closes application upon click of "X" icon
        private void Close_Icon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you Sure?", "Management", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        //allows for window to be dragged accross screen
        private void DragBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


        private async Task<bool> reset_password(string email, string user)
        {
            Sign_Inbtn.IsEnabled = false;
            EmailDetailTxtBox_S.IsEnabled = false;
            Sign_Inbtn.Content = "Sending...";

            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();

            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = "^[a-zA-Z]{9}" });
            string new_password = randomizerTextRegex.Generate();

            var send_mail = new Email_Sender("Management", email, user, "Reset Password");
            send_mail.Password_C = new_password;

            if (await Task.Run(() => send_mail.send_email(new_password)))
            {
                sQLConnectionsClass.Get_Existing_Users(new Existing_User(email, new_password), "update");
                return true;
            }
            else
            {
                return false;
            }
        }


        //authenticates user details
        private async void Sign_Inbtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (forget_password == false)
            {
                if (sign_in)
                {
                    EmailTxtBox.IsEnabled = false;
                    PasswordTxtBox.IsEnabled = false;
                    Sign_Inbtn.Content = "Loading...";
                    user_details = new Existing_User(EmailTxtBox.Text.Trim(), PasswordTxtBox.Password.Trim());


                    if (user_details.Detail_Verification(user_details))
                    {
                        EmailTxtBox.IsEnabled = true;
                        PasswordTxtBox.IsEnabled = true;
                        EmailTxtBox.Clear();
                        PasswordTxtBox.Clear();
                        Sign_Inbtn.Content = "Sign in";

                        Hide();
                        DashboardWindow dashboardWindow = new DashboardWindow
                        {
                            username_string = user_details.Existing_username
                        };

                        _ = new Home_Page
                        {
                            welcome_property = $"Welcome {user_details.Existing_username}!"
                        };
                        dashboardWindow.Show();

                    }
                    else
                    {
                        EmailTxtBox.IsEnabled = true;
                        PasswordTxtBox.IsEnabled = true;
                        EmailTxtBox.Clear();
                        PasswordTxtBox.Clear();
                        EmailTxtBox.Focus();
                        Sign_Inbtn.Content = "Try again";
                    }
                }
                else
                {
                    EmailTxtBox.IsEnabled = false;
                    EmailDetailTxtBox_S.IsEnabled = false;
                    PasswordTxtBox.IsEnabled = false;
                    Sign_Inbtn.Content = "Registering...";

                    new_user_details = new New_User(EmailTxtBox.Text.Trim(), EmailDetailTxtBox_S.Text.Trim(), PasswordTxtBox.Password.Trim());


                    if (new_user_details.username_password_validation() && new_user_details.email_validation() && SQL_Connection.Register_Information("user", new_user_details))
                    {
                        MessageBox.Show("Successful", "Management");
                        EmailTxtBox.IsEnabled = true;
                        EmailDetailTxtBox_S.IsEnabled = true;
                        PasswordTxtBox.IsEnabled = true;
                        EmailTxtBox.Clear();
                        EmailDetailTxtBox_S.Clear();
                        PasswordTxtBox.Clear();
                        Sign_Inbtn.Content = "Sign up";
                    }
                    else
                    {
                        MessageBox.Show("Invalid user details, Try again.", "Management");
                        EmailTxtBox.IsEnabled = true;
                        EmailDetailTxtBox_S.IsEnabled = true;
                        PasswordTxtBox.IsEnabled = true;
                        EmailTxtBox.Clear();
                        EmailDetailTxtBox_S.Clear();
                        PasswordTxtBox.Clear();
                        EmailTxtBox.Focus();
                        Sign_Inbtn.Content = "Try again";
                    }
                }
            }
            else
            {

                string email_d = EmailDetailTxtBox_S.Text.Trim();

                if (!String.IsNullOrEmpty(email_d))
                {
                    if (new SQLConnectionsClass().Get_Existing_Users(new Existing_User(email_d, null), "verify") && await reset_password(email_d, email_d))
                    {
                        Sign_Inbtn.Content = "Sent";
                        MessageBox.Show("Password successfully reset, details forwarded to your email.", "Management App");
                        dt.Start();
                    }
                    else
                    {
                        MessageBox.Show("Email does not exist.", "Management App");
                        EmailDetailTxtBox_S.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Field cannot be empty!");
                    EmailDetailTxtBox_S.Focus();
                }
            }


        }

        int increment_timer = 30;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Dt_Tick(object sender, EventArgs e)
        {
            if (increment_timer > 0)
            {
                increment_timer--;
                timer_interval = $"Didn't get the password?\r\nResend in: {increment_timer / 60}:{increment_timer % 60}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("timer_interval"));
            }
            else if (increment_timer == 0)
            {
                Sign_Inbtn.IsEnabled = true;
                Sign_Inbtn.Content = "Request";
                EmailDetailTxtBox_S.IsEnabled = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("timer_interval"));
                dt.Stop();
                increment_timer += 60;
            }
        }

        //changes login appearance depending on login_state
        private void Switch_To_Login(bool current_state)
        {
            if (current_state)
            {
                Sign_Up_Txt.Text = Sign_In_Txt.Text;
                Sign_Up_WrapTxt.Text = Sign_In_WrapTxt.Text;
                Email_Border.Visibility = Visibility.Visible;
                Forgot_Label.Visibility = Visibility.Collapsed;
                Sign_Upbtn.Content = "Sign in";


                Sign_In_Txt.Text = "Sign Up";
                Sign_In_WrapTxt.Text = "Sign up with your name, username and password to get started";
                Sign_Inbtn.Content = "Sign up";

                sign_in = false;
            }
            else
            {
                Sign_Up_Txt.Text = Sign_In_Txt.Text;
                Sign_Up_WrapTxt.Text = Sign_In_WrapTxt.Text;
                Email_Border.Visibility = Visibility.Collapsed;
                Forgot_Label.Visibility = Visibility.Visible;
                Sign_Upbtn.Content = "Sign up";


                Sign_In_Txt.Text = "Sign In";
                Sign_In_WrapTxt.Text = "Proceed with your username and password";
                Sign_Inbtn.Content = "Sign in";

                sign_in = true;
            }
        }


        private void Switch_to_Forget(bool current_state)
        {
            if (current_state)
            {
                Sign_Up_Txt.Text = "Return to login?";
                Sign_Up_WrapTxt.Text = "Click the button below to return to the login page.";
                Email_Border.Visibility = Visibility.Visible;
                Forgot_Label.Visibility = Visibility.Collapsed;
                Timer_Label.Visibility = Visibility.Visible;
                Sign_Upbtn.Content = "Return";


                Sign_In_Txt.Text = "Forgot Password";
                Sign_In_WrapTxt.Text = "A temporary password will be sent to your registered email.";
                Sign_Inbtn.Content = "Request";

                username_border.Visibility = Visibility.Collapsed;
                password_border.Visibility = Visibility.Collapsed;
            }
            else
            {
                Switch_To_Login(true);
                Switch_To_Login(false);

                EmailDetailTxtBox_S.Clear();
                increment_timer = 30;
                timer_interval = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("timer_interval"));
                username_border.Visibility = Visibility.Visible;
                password_border.Visibility = Visibility.Visible;
                Timer_Label.Visibility = Visibility.Collapsed;

            }
        }


        private void Sign_Upbtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (forget_password)
            {
                Switch_to_Forget(false);
                forget_password = false;
            }
            else
            {
                Switch_To_Login(sign_in);
            }
        }

        private void Forgot_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            forget_password = true;
            Switch_to_Forget(forget_password);
        }
    }
}

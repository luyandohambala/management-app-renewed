using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace management_app_renewed
{
    public partial class Home_Page : Page
    {
        Room_Management_Page load_data = new Room_Management_Page();
        Students_Management_Page student_data = new Students_Management_Page();
        Payment_Management_Page payment_data = new Payment_Management_Page();

        public string welcome_property { get; set; }
        public string total_rooms_property { get; set; }
        public string total_occupants_property { get; set; }
        public string total_rentals_property { get; set; }
        public Home_Page()
        {
            InitializeComponent();
            DataContext = this;
            load_template_data();
        }

        //load template data
        private void load_template_data()
        {
            load_data.prepare_data("None");
            student_data.prepare_data("None");
            payment_data.prepare_data("Outstanding Balance");

            total_rooms_property = $"Total Room(s) \r\navailable: {load_data.room_Models.Count}";

            total_occupants_property = $"Total Occupant(s):\r\n{student_data.occupant_Models.Count}";
            
            total_rentals_property = $"Resident(s) W/Pending \r\nrentals: {payment_data.payment_count.Count}";
        }

        /*private void total_rooms_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dashboardWindow.Search_Grid.Visibility = Visibility.Visible;         
            load_data.navigate_rooms("view");
            dashboardWindow.Display_Window.Content = load_data;
            

        }*/
    }
}

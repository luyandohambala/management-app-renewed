namespace management_app_renewed
{
    public class Payments
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public int Room_Number { get; set; }
        public string Room_Type { get; set; }
        public string Gender_P { get; set; }
        public string Amount_Due { get; set; }
        public string Amount_Paid { get; set; }
        public string Date_Paid { get; set; }
        public string Date_Due { get; set; }

        public Payments()
        {

        }

        public Payments(string first, string last, int num, string room_t, string amound_d, string amount_p, string date_p, string date_d)
        {
            First_Name = first;
            Last_Name = last;
            Room_Number = num;
            Room_Type = room_t;
            Amount_Due = amound_d;
            Amount_Paid = amount_p;
            Date_Paid = date_p;
            Date_Due = date_d;
        }
        

        public bool submit_payment(string type, Payments payments)
        {
            SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
            //return true if room successfully created;
            if (SQL_Connection.submit_Payment(type, payments))
            {
                return true;
            }
            return false;
        }


        public List<Payments> GetPayments()
        {
            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
            
            return sQLConnectionsClass.Load_Payments();
        }
    }
}

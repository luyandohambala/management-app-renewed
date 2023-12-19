using System;
using System.Collections.Generic;
using System.Configuration;

namespace management_app_renewed
{
    internal class Settings: ConfigurationSection
    {
        public string Company_Name { get; set; }
        
        public int Floor_Number { get; set; }

        public string Room_Allocation { get; set; }
        
        //currency setting
        public char Currency { get; set; }

        //price setting for hotel
        public string HTsingle_amount { get; set; }
        
        public string HTdouble_amount {get; set; }
        
        public string HBfour_amount { get; set; }

        public Settings(string c_n, int f_n, string rm, char c, string hts, string htd, string hbf)
        {
            Company_Name = c_n;
            Floor_Number = f_n;
            Room_Allocation = rm;
            Currency = c;
            HTsingle_amount = hts;
            HTdouble_amount = htd;
            HBfour_amount = hbf;
        }

        public Settings()
        {
            
        }


        public bool save_settings(Settings settings)
        {
            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
            if (sQLConnectionsClass.register_settings(settings))
            {
                return true;
            }
            return false;
        }

        public List<Settings> load_settings()
        {
            SQLConnectionsClass sQLConnectionsClass = new SQLConnectionsClass();
            if (sQLConnectionsClass.load_settings() != null)
            {
                return sQLConnectionsClass.load_settings();
            }
            return null;
        }

    }
}

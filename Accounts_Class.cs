using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace management_app_renewed
{
    public class Accounts_Class
    {

    }

    public class Existing_User : Accounts_Class
    {
        public string Existing_username { get; set; }
        public string Existing_password { get; set; }

        public Existing_User(string username, string password)
        {
            Existing_username = username;
            Existing_password = password;
        }

        //pass created object to sql_connection class
        SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();

        public Existing_User()
        {
                
        }

        //function verifys user details
        public bool Detail_Verification(Existing_User user_details)
        {
            //compare with database values
            if (SQL_Connection.Load_Information(user_details))
            {
                return true;
            }
            return false;
        }
    }

    public class New_User : Accounts_Class 
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public New_User()
        {
            
        }

        //function verifies that values passed for username and password are of correct format
        public New_User(string username, string email, string password) 
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public bool username_password_validation()
        {
            Regex usercheckchar = new Regex("[^a-zA-Z0-9]");
            Regex usercheckUppc = new Regex("[A-Z0-9]");
            Regex userchecklowC = new Regex("[a-z0-9]");
            Regex passchecklowC = new Regex("[a-z]");
            Regex passcheckuppC = new Regex("[A-Z]");

            if (passchecklowC.IsMatch(Password) && (passcheckuppC.IsMatch(Password))
                && (!usercheckchar.IsMatch(Username)) && (usercheckUppc.IsMatch(Username)) && (userchecklowC.IsMatch(Username)))
            {
                return true;
            }
            return false;
        }

        //function verifies that values passed for email is of the correct format
        public bool email_validation()
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(Email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }

    public class Room_Allocator
    {
        public int Room_Number {get; set;}
        public string Room_Type {get; set;}
        public string Floor_Number {get; set;}
        public int Occupants { get; set;}
        public string Gender_Type {get; set;}
        public string Cost {get; set;}

        //initiallizes properties via constructor below

        public Room_Allocator()
        {
                
        }

        public Room_Allocator(int room, string room_type, string floor_num, int occu_num, string gender, string cost)
        {
            Room_Number = room;
            Room_Type = room_type;
            Floor_Number = floor_num;
            Occupants = occu_num;
            Gender_Type = gender;
            Cost = cost;
        }

        //pass created object to sql_connection class
        SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
        public bool create_room(string to_do, Room_Allocator room_Allocator)
        {
            //return true if room successfully created;
            if (SQL_Connection.Register_Room_Information(to_do, room_Allocator))
            {
                return true;
            }
            return false;
        }
    }

    public class Student_Allocator
    {
        public string First_Name { get; set;}
        
        public string Last_Name { get; set;}

        public string Gender { get; set; }

        public int Room_Number { get; set; }

        public Student_Allocator()
        {
                
        }

        //initialise above properties using constructor below
        public Student_Allocator(string first, string last, string gender, int room)
        {
            First_Name = first;
            Last_Name = last;
            Gender = gender;
            Room_Number = room;
        }


        //pass created object to sql_connection class
        SQLConnectionsClass SQL_Connection = new SQLConnectionsClass();
        public bool create_student(Student_Allocator student_Allocator)
        {
            //return true if student successfully added
            if (SQL_Connection.Register_Information("student", student_Allocator))
            {
                return true;
            }
            return false;
            
        }


    }


    public class Email_Sender
    {
        public string Sender { get; set;}

        public string Reciever { get; set; }
        
        public string User { get; set; }

        public string Subject { get; set; }

        public string Password_C { get; set; }


        public Email_Sender(string sender, string rec, string user, string sub)
        {
            Sender = sender;
            Reciever = rec;
            User = user;
            Subject = sub;
        }

        
        private StringBuilder stringBuilder(string to_do)
        {
            if (to_do == "reset password")
            {
                StringBuilder template = new StringBuilder();
                template.AppendLine("Dear @Model.FirstName,");
                template.AppendLine("<p>We've noticed you're trying to reset your password. As requested we've generated one for you.</p>");
                template.AppendLine("<p>Here it is <strong>@Model.Password</strong>");
                template.AppendLine("- Management.");

                return template;
            }
            else
            {
                StringBuilder template = new StringBuilder();
                template.AppendLine($"Dear {User},\r\n");
                template.AppendLine("We've noticed you're trying to reset your password. As requested we've generated one for you.");
                template.AppendLine($"Here it is;\r\n");
                template.AppendLine($"{Password_C}\r\n");
                template.AppendLine("\r\n- Management.");
                return template;
            }
            
        }
        
        public bool send_email(string to_send)
        {
            var sender = new SmtpSender(() => new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("luyandohambala240@gmail.com", "wnadmobvunjixrjk"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            });

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();
            
            
                var email = Email
                .From("luyandohambala240@gmail.com", Sender)
                .To(Reciever)
                .Subject(Subject)
                .UsingTemplate(stringBuilder("reset password").ToString(), new {FirstName = User, Password = Password_C})
                //.Body(stringBuilder("mo").ToString())
                .SendAsync();
            return true;

        }
    }
}

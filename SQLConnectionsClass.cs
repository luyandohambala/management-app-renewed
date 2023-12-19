using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace management_app_renewed
{
    internal class SQLConnectionsClass
    {
        //load sqlite databse connection
        public string Connect(string id = "Default")
        {
            /*var builder = new ConfigurationBuilder();
            
            IConfigurationRoot configuration = builder.Build();
            return configuration.GetConnectionString("Default");*/
            return System.Configuration.ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


        //delete info from database
        public bool delete_info(string table_name, string id, string id1, int num, string id2)
        {
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {
                if (table_name == "rooms")
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "Delete From Rooms where (Room_Number = @id)";
                        command.Parameters.Add(new SQLiteParameter("@id", id));
                        command.ExecuteNonQuery();
                        db.Close();
                        return true;
                    }   
                }
                else if (table_name == "students")
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "Delete From Residents where (First_Name = @F_N AND Last_Name = @L_N AND Room_Number = @R_N)";
                        command.Parameters.Add(new SQLiteParameter("@F_N", id));
                        command.Parameters.Add(new SQLiteParameter("@L_N", id1));
                        command.Parameters.Add(new SQLiteParameter("@R_N", num));
                        command.ExecuteNonQuery();

                        db.Close();
                        return true;
                    }

                }
                else if (table_name == "payment")
                {
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "Delete From Payments where (First_Name = @F_N AND Last_Name = @L_N AND Date_Paid = @D_P)";
                        command.Parameters.Add(new SQLiteParameter("@F_N", id));
                        command.Parameters.Add(new SQLiteParameter("@L_N", id1));
                        command.Parameters.Add(new SQLiteParameter("@D_P", id2));
                        command.ExecuteNonQuery();

                        db.Close();
                        return true;
                    }
                }
                else if (table_name == "users")
                {
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "Delete From Users where (Username = @F_N AND Email = @L_N AND Password = @D_P)";
                        command.Parameters.Add(new SQLiteParameter("@F_N", id));
                        command.Parameters.Add(new SQLiteParameter("@L_N", id1));
                        command.Parameters.Add(new SQLiteParameter("@D_P", id2));
                        command.ExecuteNonQuery();

                        db.Close();
                        return true;
                    }
                }
                else if(table_name == "delete all")
                {
                    db.Execute("Delete From Residents");
                    db.Execute("Delete From Rooms");
                    return true;
                }
                return false;
            }
        }


        //load information to databse
        public bool Load_Information(Existing_User user_details)
        {
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<Existing_User>("Select Username, Password From Users where (Username = @Existing_username and Password = @Existing_password)", user_details);
                if ((result.Count() > 0))
                {
                    return true;
                }
                return false;
                
            }
        }

        public bool Get_Existing_Users(Existing_User user, string to_do)
        {
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {
                if (to_do == "verify")
                {
                    var result = db.Query<New_User>("Select Email From Users where (Email = @Existing_username)", user);
                    if ((result.Count() > 0))
                    {
                        return true;
                    }
                    return false;
                }
                else if (to_do == "update")
                {
                    db.Execute(@"UPDATE Users SET Password = @Existing_password 
                            WHERE Email = @Existing_username", user);
                    return true;
                }
                return false;
            }
        }

        public List<New_User> load_user_information()
        {
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<New_User>("select * from users");
                return result.ToList();
            }
        }

        public bool edit_users(New_User existing_User)
        {
            try
            {
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {

                    db.Execute(@"UPDATE Users SET Username = @Username, 
                                                Email = @Email, 
                                                Password = @Password 
                            WHERE Username = @Username", existing_User);

                    return true;
                }
            }
            catch (SQLiteException exception)
            {
                MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                return false;
            }
        }

        public List<Room_Allocator> Load_Room_Information()
        {
            using (var db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<Room_Allocator>(@"SELECT R.Room_Number, R.Room_Type, R.Floor_Number, Count(O.Room_Number) as Occupants, R.Gender_Type, R.Cost 
                                                        FROM Rooms as R left JOIN Residents as O ON R.Room_Number = O.Room_Number GROUP by R.Room_Number;");
                return result.ToList();
            }

        }
        
        public List<Student_Allocator> Load_Occupant_Information()
        {
            using (var db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<Student_Allocator>("Select * From Residents");
                return result.ToList();
            }

        }

        public List<Payments> Load_Payments()
        {
            using (var db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<Payments>("Select * From Payments");
                return result.ToList();
            }
        }

        //save informtion to database
        public bool Register_Information(string to_do, object user_details)
        {
            
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {
                    try
                    {
                        if (to_do == "user")
                        {
                            db.Execute("insert into Users values (@Username, @Email, @Password)", user_details);

                            return true;
                        }
                        else if (to_do == "student")
                        {
                            db.Execute("insert into Residents values (@First_Name, @Last_Name, @Gender, @Room_Number)", user_details);

                            return true;
                        }
                        return false;

                    }
                    catch (SQLiteException exception)
                    {
                        if (exception.Message.Contains("constraint failed"))
                        {
                            MessageBox.Show("Username already in use.", "Management Application");
                            return false;
                        }
                        else
                        {
                            MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                            return false;
                        }
                    }
                }
            
        }

        
        //adds or edits room in application databse
        public bool Register_Room_Information(string to_do, Room_Allocator room_Allocator)
        {
            try
            {
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {

                    if (to_do == "submit")
                    {
                        db.Execute("insert into Rooms values (@Room_Number, @Room_Type, @Floor_Number, @Gender_Type, @Cost)", room_Allocator);
                        return true;
                    }
                    else if (to_do == "edit")
                    {
                        db.Execute(@"UPDATE Rooms SET Room_Number = @Room_Number, 
                                                    Room_Type = @Room_Type, 
                                                    Floor_Number = @Floor_Number,
                                                    Gender_Type = @Gender_Type
                               WHERE Room_Number = @Room_Number", room_Allocator);
                        
                        return true;
                    }
                    return false;

                }
            }
            catch (SQLiteException exception)
            {
                MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                return false;
            }
        }

        public bool Edit_Occupant_Information(Student_Allocator student_Allocator)
        {
            try
            {
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {

                    db.Execute(@"UPDATE Residents SET First_Name = @First_Name, 
                                                Last_Name = @Last_Name, 
                                                Gender_Type = @Gender_Type, 
                                                Room_Number = @Room_Number
                            WHERE (First_Name = @First_Name AND Last_Name = @Last_Name)", student_Allocator);
                        
                    return true;
                }
            }
            catch (SQLiteException exception)
            {
                MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                return false;
            }
        }

        public bool submit_Payment(string type, Payments payments)
        {
            if (type == "submit")
            {
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {
                    try
                    {
                        db.Execute(@"insert into Payments values 
                                    (@First_Name, 
                                    @Last_Name,
                                    @Room_Number,
                                    @Room_Type,
                                    @Amount_Due,
                                    @Amount_Paid,
                                    @Date_Paid,
                                    @Date_Due
                                    )", payments);

                        return true;

                    }
                    catch (SQLiteException exception)
                    {
                        MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                        return false;
                    }
                }
            }
            else if (type == "renew")
            {
                using (IDbConnection db = new SQLiteConnection(Connect()))
                {
                    try
                    {
                        db.Execute(@"UPDATE Payments SET First_Name = @First_Name,
                                                Last_Name = @Last_Name, 
                                                Room_Number = @Room_Number, 
                                                Room_Type = @Room_Type,
                                                Amount_Due = @Amount_Due,
                                                Amount_Paid = @Amount_Paid,
                                                Date_Paid = @Date_Paid,
                                                Date_Due = @Date_Due
                            WHERE (First_Name = @First_Name AND Last_Name = @Last_Name)", payments);

                        return true;

                    }
                    catch (SQLiteException exception)
                    {
                        MessageBox.Show($"Error code: {exception.Message}.", "Management Application");
                        return false;
                    }
                }
            }
            return false;
        }

        //configure user settings
        public bool register_settings(Settings settings)
        {
            
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {

                db.Execute(@"UPDATE Settings SET Company_Name = @Company_Name,
                                    Floor_Number = @Floor_Number,
                                    Room_Allocation = @Room_Allocation,
                                    Currency = @Currency,
                                    HTsingle_amount = @HTsingle_amount,
                                    HTdouble_amount = @HTdouble_amount,
                                    HBfour_amount = @HBfour_amount
                            ", settings);
                return true;
            }
            
        }

        public List<Settings> load_settings()
        {
            using (IDbConnection db = new SQLiteConnection(Connect()))
            {
                var result = db.Query<Settings>("Select * From Settings");
                return result.ToList();
            }
        }
    }
}

using Coding_Tracker.SQL;
using ConsoleTableExt;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Configuration;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Coding_Tracker
{
    internal class Program
    {
        public static SQLite SQLite = new SQLite();
        public static SqliteConnection connection = SQLite.CreateConnection();

        public static void Main(string[] args)
        {
            SQLite.CreateTable(
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "StartTime TEXT," +
                "EndTime TEXT," +
                "Duration TEXT");

            ShowMenu();
            connection.Close();
        }

        static public void ShowMenu()
        {
            Console.Clear();
            Console.Write($"-------------------\n" +
                $"0. Quit the application\n" +
                $"1. Add a coding session\n" +
                $"2. Start a session via stopwatch\n" +
                $"3. List all sessions\n" +
                $"-------------------\n" +
                $"Enter the number: ");
            GetMenuChoice();
        }

        public static void GetMenuChoice()
        {
            int choice = GetNumberInput();
            switch (choice)
            {
                case 0:
                    break;
                case 1:
                    Console.Clear();
                    DateTime startDate = GetDate("Enter the start date or 0 to go back:");
                    DateTime endDate = GetDate("Enter the end date or 0 to go back:");
                    var duration = endDate - startDate;
                    if (startDate > endDate)
                    {
                        Console.WriteLine("Start date must be before the end date!\nTry again!");
                    }
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = 
                        @$"INSERT INTO {SQLite.DatabaseName}(StartTime, EndTime, Duration) 
                        VALUES('{startDate.ToString("dd-MM-yy HH:mm:ss")}','{endDate.ToString("dd-MM-yy HH:mm:ss")}','{duration.Hours}:{duration.Minutes}:{duration.Seconds}')";
                    if (cmd.ExecuteNonQuery() > 0)
                        Console.WriteLine("\n\nSuccessfully inserted new entry");
                    else
                        Console.WriteLine("Failed to insert a new entry.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    ShowMenu();
                    break;
                case 2:
                    Console.Clear();
                    DateTime startDateRealTime = DateTime.Now;
                    Console.Write($"Start time is {startDateRealTime}\nPress any button to finish:");
                    Console.ReadKey();
                    DateTime endDateRealTime = DateTime.Now;
                    Console.WriteLine($"Finished at {endDateRealTime}");
                    var durationRealTime = endDateRealTime - startDateRealTime;
                    var cmdRealTime = connection.CreateCommand();
                    cmdRealTime.CommandText =
                        @$"INSERT INTO {SQLite.DatabaseName}(StartTime, EndTime, Duration) 
                        VALUES('{startDateRealTime.ToString("dd-MM-yy HH:mm:ss")}','{endDateRealTime.ToString("dd-MM-yy HH:mm:ss")}','{durationRealTime.Hours}:{durationRealTime.Minutes}:{durationRealTime.Seconds}')";
                    if (cmdRealTime.ExecuteNonQuery() > 0)
                        Console.WriteLine("\n\nSuccessfully inserted new entry");
                    else
                        Console.WriteLine("Failed to insert a new entry.");
                    Console.WriteLine("To get to menu press any key...");
                    Console.ReadLine();
                    ShowMenu();
                    break;
                case 3:
                    Console.Clear();
                    ConsoleTableBuilder.From(SQLite.GetAllEntries()).ExportAndWriteLine();
                    Console.WriteLine("To get to menu press any key...");
                    Console.ReadLine();
                    ShowMenu();
                    break;
                default:
                    Console.Clear();
                    Console.Write("Wrong input! Please, choose the number between 0 and 3.\n" +
                        "Press any key to continue...");
                    Console.ReadKey();
                    ShowMenu();
                    break;
            }
        }

        public int GetUserInput()
        {
            string temp = Console.ReadLine();
            while (!int.TryParse(temp, out var _))
            {
                Console.WriteLine("Wrong input!");
                temp = Console.ReadLine();
            }
            return int.Parse(temp);
        }

        private static DateTime GetDate(string message)
        {
            Console.Write(message + " (dd-mm-yy hh:mm:ss) ");
            string sDate = Console.ReadLine();
            while(!DateTime.TryParseExact(sDate, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
            {
                if (sDate == "0")
                    ShowMenu();
                Console.WriteLine("Invalid date! Format: dd-mm-yy hh:mm:ss\nTry again:");
                sDate = Console.ReadLine();
            }
            var DTDate = DateTime.ParseExact(sDate, "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
            return DTDate;
        }

        static private int GetNumberInput()
        {
            var s = Console.ReadLine();
            while(!int.TryParse(s, out var n))
            {
                Console.WriteLine("Wrong input!");
                s = Console.ReadLine();
            }
            return Convert.ToInt32(s);
        }
    }
}
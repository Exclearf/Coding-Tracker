using Coding_Tracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding_Tracker.SQL
{
    internal class SQLite : ISQLite
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public SqliteConnection connection { get; set; }

        public SQLite()
        {
            ConnectionString = ConfigurationManager.AppSettings["SQLPath"];
            DatabaseName = ConnectionString.Split("=")[1].Split(".")[0];
        }

        public SqliteConnection CreateConnection()
        {
            connection = new SqliteConnection(ConnectionString);
                try
                {
                    connection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured when opening a database!");
                    Console.WriteLine(e);
                }
                return connection;
        }

        public int ExecuteCommand(string cmd)
        {
            var command = connection.CreateCommand();
            command.CommandText = cmd;
            return command.ExecuteNonQuery();
        }

        public void CreateTable(string content) {
            string table =
                @$"CREATE TABLE IF NOT EXISTS {DatabaseName} (
                {content}
                )";
            if (ExecuteCommand(table) == -1)
                Console.WriteLine("Error occured while creating the table");
        }

        public List<CodingSession> GetAllEntries()
        {
            List<CodingSession> result = new();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {DatabaseName}";

            SqliteDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while(reader.Read())
                {
                    result.Add(new CodingSession
                    {
                        ID = reader.GetInt32(0),
                        StartTime = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture),
                        EndTime = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture),
                        Duration = DateTime.ParseExact(reader.GetString(3).Split(" ").Last(), "H:m:s", CultureInfo.InvariantCulture).ToString("HH:mm:ss")
                    });
                }
            }
            else
                Console.WriteLine("No records!");
            return result;
        }
    }
}

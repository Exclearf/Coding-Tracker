using Coding_Tracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding_Tracker.SQL
{
    internal interface ISQLite
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public SqliteConnection CreateConnection();
        public int ExecuteCommand(string cmd);
        public void CreateTable(string content);
        public List<CodingSession> GetAllEntries();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KORT.Database
{
    public class DatabaseManager
    {
        IDatabase _database;

        public DatabaseManager(IDatabase database)
        {
            _database = database;
        }

        public void Search(string commandText)
        {
            _database.Select(commandText);
        }

        public void Add(string commandText)
        {
            _database.Insert(commandText);                
        }

        public void Save(string commandText)
        {
            _database.Update(commandText);
        }

        public void Remove(string commandText)
        {
            _database.Delete(commandText);
        }
    }
}

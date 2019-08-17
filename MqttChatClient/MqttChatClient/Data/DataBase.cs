using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;

namespace MqttChatClient.Models
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection _database;

        public DataBase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Message>().Wait();
        }

        public Task<int> SaveItemAsync(Message message)
        {
            if (message.Id != 0)
            {
                return _database.UpdateAsync(message);
            }
            else
            {
                return _database.InsertAsync(message);
            }
        }

        public Task<List<Message>> GetItemsAsync()
        {
            return _database.Table<Message>().ToListAsync();
        }

        public Task<List<MessageWrapper>> GetMessagesAsync(string sender, string receiver)
        {
            string participants = string.Format("(\"{0}\",\"{1}\")", sender, receiver);
            return _database.QueryAsync<MessageWrapper>("SELECT * FROM [Message] WHERE [Sender] in " + participants + " " +
                "and [Receiver] in " + participants + "" +
                "order by [CreatedTime] asc " 
                //"limit 10 " 
                );
        }

        public Task<List<Message>> GetLatestMessagges()
        {
            string query = "WITH summary AS ( " +
                "SELECT m.*, " +
                "ROW_NUMBER() OVER(PARTITION BY m.Sender, m.Receiver " +
                "ORDER BY m.CreatedTime DESC) AS rk " +
                "FROM Message m) " +
                "SELECT * " +
                "FROM summary s " +
                "WHERE s.rk = 1 " +
                "ORDER BY s.CreatedTime DESC";
            return _database.QueryAsync<Message>(query);
        }

    }
}

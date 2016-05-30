using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace XamarinSQLite.PCL
{
    public class TodoRepository
    {
        private SQLiteAsyncConnection _connection;

        public async Task InitializeAsync(string path, ISQLitePlatform sqlitePlatform)
        {
            _connection = SQLiteDatabase.GetConnection(path, sqlitePlatform);

            // Create MyEntity table if need be
            await _connection.CreateTableAsync<TodoItem>();
        }

        public async Task<TodoItem> CreateAsync(string text)
        {
            var entity = new TodoItem()
            {
                Text = text
            };
            var count = await _connection.InsertAsync(entity);
            return (count == 1) ? entity : null;
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            var entities = await _connection.Table<TodoItem>().ToListAsync();
            return entities;
        }
    }
}

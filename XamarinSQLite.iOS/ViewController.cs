using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;
using SQLite.Net.Platform.XamarinIOS;
using UIKit;
using XamarinSQLite.PCL;

namespace XamarinSQLite.iOS
{
    public partial class ViewController : UIViewController
    {
        private TodoRepository _todoRepository;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        private string GetDbPath()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(documentsPath, "..", "Library", "Todo.db3");
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddButton.TouchUpInside += OnAddButtonTouchUpInside;
            
            _todoRepository = new TodoRepository();

            var path = GetDbPath();

            await _todoRepository.InitializeAsync(path, new SQLitePlatformIOS());
            var items = await _todoRepository.GetAllAsync();
            
            TodoTable.Source = new TodoTableDataSource(items, TodoTable);
            TodoTable.ReloadData();
        }

        private async void OnAddButtonTouchUpInside(object sender, EventArgs e)
        {
            var text = TodoTextField.Text;
            TodoTextField.Text = string.Empty;

            var todoItem = await _todoRepository.CreateAsync(text);

            var source = TodoTable.Source as TodoTableDataSource;
            source.Add(todoItem);
        }
    }

    public class TodoTableDataSource : UITableViewSource
    {
        private List<TodoItem> _items;
        private string _cellIdentifier = "TableCell";
        private UITableView _tableView;

        public TodoTableDataSource(IEnumerable<TodoItem> items, UITableView tableView)
        {
            _items = items.ToList();
            _tableView = tableView;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _items.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier);
            var todo = _items[indexPath.Row];

            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);

            cell.TextLabel.Text = todo.Text;

            return cell;
        }

        public void Add(TodoItem item)
        {
            _items.Add(item);
            _tableView.ReloadData();
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using SQLite.Net.Platform.XamarinAndroid;
using XamarinSQLite.PCL;

namespace XamarinSQLite.Android
{
    [Activity(Label = "XamarinSQLite.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private TodoRepository _todoRepository;

        private string GetDbPath()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(documentsPath, "Todo.db3");
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var addButton = FindViewById<Button>(Resource.Id.AddButton);
            var todoListView = FindViewById<ListView>(Resource.Id.TodoListView);
            addButton.Click += OnAddButtonClicked;

            _todoRepository = new TodoRepository();

            var path = GetDbPath();

            await _todoRepository.InitializeAsync(path, new SQLitePlatformAndroid());
            var items = await _todoRepository.GetAllAsync();

            todoListView.Adapter = new ArrayAdapter<string>(this, global::Android.Resource.Layout.SimpleListItem1, items.Select(i => i.Text).ToList());
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var todoEditText = FindViewById<EditText>(Resource.Id.TodoEditText);
            var text = todoEditText.Text;
            todoEditText.Text = string.Empty;

            var todoItem = await _todoRepository.CreateAsync(text);
            
            var todoListView = FindViewById<ListView>(Resource.Id.TodoListView);
            var adapter = todoListView.Adapter as ArrayAdapter<string>;
            adapter.Add(todoItem.Text);
        }
    }
}
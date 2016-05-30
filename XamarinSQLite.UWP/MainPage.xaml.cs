using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XamarinSQLite.PCL;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace XamarinSQLite.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TodoRepository _todoRepository;
        public ObservableCollection<TodoItem> Items { get; set; } = new ObservableCollection<TodoItem>();

        public MainPage()
        {
            this.InitializeComponent();
        }

private async Task<string> GetDbPathAsync()
{
    var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
    var file = await folder.CreateFileAsync("Todo.db3", CreationCollisionOption.OpenIfExists);
    return file.Path;
}

protected override async void OnNavigatedTo(NavigationEventArgs e)
{
    _todoRepository = new TodoRepository();

    var path = await GetDbPathAsync();

    await _todoRepository.InitializeAsync(path, new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT());

    var items = await _todoRepository.GetAllAsync();
    foreach (var todoItem in items)
    {
        Items.Add(todoItem);
    }
}

private async void OnAddTodoItemButtonClicked(object sender, RoutedEventArgs e)
{
    var text = TodoTextBox.Text;
    TodoTextBox.Text = string.Empty;

    var item = await _todoRepository.CreateAsync(text);
    Items.Add(item);
}
    }
}

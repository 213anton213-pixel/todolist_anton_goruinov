using System.Windows;
using TodoList.Repositories;
using TodoList.Services;

namespace TodoList;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ITaskRepository repository = new JsonTaskRepository(
            System.IO.Path.Combine(AppContext.BaseDirectory, "tasks.json"));
        var window = new MainWindow(new TodoService(), repository);
        MainWindow = window;
        window.Show();
    }
}

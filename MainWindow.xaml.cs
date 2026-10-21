using System.IO;
using System.Windows;
using System.Windows.Controls;
using TodoList.Models;
using TodoList.Repositories;
using TodoList.Services;

namespace TodoList;

public partial class MainWindow : Window
{
    private readonly TodoService _service;
    private readonly ITaskRepository _repository;

    public MainWindow(TodoService service, ITaskRepository repository)
    {
        _service = service;
        _repository = repository;
        InitializeComponent();
        Refresh();
        ClearForm();
        Loaded += (_, _) =>
        {
            if (_repository.Exists && MessageBox.Show(this,
                    "Загрузить ранее сохранённые задачи?", "Todo List",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                LoadTasks();
        };
    }

    private void Refresh(Guid? selectedId = null)
    {
        TaskList.ItemsSource = null;
        TaskList.ItemsSource = _service.Tasks;
        TaskList.SelectedItem = _service.Tasks.FirstOrDefault(task => task.Id == selectedId);
        InfoText.Text = $"Задач: {_service.Tasks.Count} · файл tasks.json находится рядом с приложением";
    }

    private void ClearForm()
    {
        TitleInput.Clear();
        DescriptionInput.Clear();
        DueDateInput.SelectedDate = DateTime.Today;
        PriorityInput.SelectedIndex = 1;
        StatusInput.SelectedIndex = 0;
        CreatedAtText.Text = "Новая задача";
    }

    private void TaskSelected(object sender, SelectionChangedEventArgs e)
    {
        if (TaskList.SelectedItem is not TodoModel task)
            return;

        TitleInput.Text = task.Title;
        DescriptionInput.Text = task.Description;
        DueDateInput.SelectedDate = task.DueDate;
        PriorityInput.SelectedIndex = (int)task.Priority;
        StatusInput.SelectedIndex = (int)task.Status;
        CreatedAtText.Text = $"Создана: {task.CreatedAt:dd.MM.yyyy HH:mm}";
    }

    private DateTime DueDate => DueDateInput.SelectedDate
        ?? throw new ArgumentException("Выбери срок выполнения.");

    private void NewClick(object sender, RoutedEventArgs e)
    {
        TaskList.SelectedItem = null;
        ClearForm();
    }

    private void AddClick(object sender, RoutedEventArgs e) => Run(() =>
    {
        var task = _service.AddTask(TitleInput.Text, DescriptionInput.Text, DueDate,
            (TaskPriority)PriorityInput.SelectedIndex, (Models.TaskStatus)StatusInput.SelectedIndex);
        Refresh(task.Id);
    });

    private void EditClick(object sender, RoutedEventArgs e) => Run(() =>
    {
        if (TaskList.SelectedItem is not TodoModel task)
            throw new ArgumentException("Сначала выбери задачу из списка.");

        _service.EditTask(task.Id, TitleInput.Text, DescriptionInput.Text, DueDate,
            (TaskPriority)PriorityInput.SelectedIndex);
        _service.ChangeStatus(task.Id, (Models.TaskStatus)StatusInput.SelectedIndex);
        Refresh(task.Id);
    });

    private void DeleteClick(object sender, RoutedEventArgs e) => Run(() =>
    {
        if (TaskList.SelectedItem is not TodoModel task)
            throw new ArgumentException("Сначала выбери задачу из списка.");
        _service.DeleteTask(task.Id);
        Refresh();
        ClearForm();
    });

    private void SaveClick(object sender, RoutedEventArgs e) => Run(() =>
    {
        _repository.Save(_service.Tasks);
        InfoText.Text = "Задачи сохранены в tasks.json";
    });

    private void LoadClick(object sender, RoutedEventArgs e) => LoadTasks();

    private void LoadTasks() => Run(() =>
    {
        if (!_repository.Exists)
            throw new FileNotFoundException("Файл tasks.json ещё не создан.");
        _service.ReplaceTasks(_repository.Load());
        Refresh();
        ClearForm();
    });

    private void Run(Action action)
    {
        try { action(); }
        catch (Exception error)
        {
            MessageBox.Show(this, error.Message, "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}

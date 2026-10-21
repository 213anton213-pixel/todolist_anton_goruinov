using System.IO;
using TodoList.Models;
using TaskStatus = TodoList.Models.TaskStatus;

namespace TodoList.Services;

public sealed class TodoService
{
    private readonly List<TodoModel> _tasks = new();
    public IReadOnlyList<TodoModel> Tasks => _tasks.AsReadOnly();

    public TodoModel AddTask(string title, string description, DateTime dueDate,
        TaskPriority priority, TaskStatus status)
    {
        var task = new TodoModel(title, description, dueDate, priority);
        task.ChangeStatus(status);
        _tasks.Add(task);
        return task;
    }

    public void EditTask(Guid id, string title, string description, DateTime dueDate,
        TaskPriority priority) => Find(id).Edit(title, description, dueDate, priority);

    public void ChangeStatus(Guid id, TaskStatus status) => Find(id).ChangeStatus(status);
    public void DeleteTask(Guid id) => _tasks.Remove(Find(id));

    private TodoModel Find(Guid id) => _tasks.FirstOrDefault(task => task.Id == id)
        ?? throw new KeyNotFoundException("Задача не найдена.");

    public void ReplaceTasks(IEnumerable<TodoModel> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        var list = tasks.ToList();
        if (list.Any(task => task is null) || list.Select(task => task.Id).Distinct().Count() != list.Count)
            throw new InvalidDataException("В файле есть пустые или повторяющиеся задачи.");
        _tasks.Clear();
        _tasks.AddRange(list);
    }
}

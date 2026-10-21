using System.Text.Json.Serialization;

namespace TodoList.Models;

public sealed class TodoModel
{
    public Guid Id { get; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; }
    public DateTime DueDate { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskStatus Status { get; private set; }

    public TodoModel(string title, string description, DateTime dueDate, TaskPriority priority)
        : this(Guid.NewGuid(), title, description, DateTime.Now, dueDate,
            priority, TaskStatus.NotStarted) { }

    [JsonConstructor]
    public TodoModel(Guid id, string title, string description, DateTime createdAt,
        DateTime dueDate, TaskPriority priority, TaskStatus status)
    {
        if (id == Guid.Empty || createdAt == default)
            throw new ArgumentException("Неверный идентификатор или дата создания.");

        Id = id;
        CreatedAt = createdAt;
        Edit(title, description, dueDate, priority);
        ChangeStatus(status);
    }

    public void Edit(string title, string description, DateTime dueDate, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 120)
            throw new ArgumentException("Название должно содержать от 1 до 120 символов.");
        ArgumentNullException.ThrowIfNull(description);
        if (description.Length > 2000)
            throw new ArgumentException("Описание не должно превышать 2000 символов.");
        if (dueDate == default || dueDate.Date < CreatedAt.Date)
            throw new ArgumentException("Срок не может быть раньше даты создания.");
        if (!Enum.IsDefined(priority))
            throw new ArgumentException("Неверный приоритет.");

        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate.Date;
        Priority = priority;
    }

    public void ChangeStatus(TaskStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentException("Неверный статус.");
        Status = status;
    }
}

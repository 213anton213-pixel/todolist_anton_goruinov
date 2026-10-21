using TodoList.Models;

namespace TodoList.Repositories;

public interface ITaskRepository
{
    bool Exists { get; }
    IReadOnlyList<TodoModel> Load();
    void Save(IEnumerable<TodoModel> tasks);
}

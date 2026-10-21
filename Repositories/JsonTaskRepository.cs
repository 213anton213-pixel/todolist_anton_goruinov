using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoList.Models;

namespace TodoList.Repositories;

public sealed class JsonTaskRepository : ITaskRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _path;

    public JsonTaskRepository(string path) => _path = path;
    public bool Exists => File.Exists(_path);

    public IReadOnlyList<TodoModel> Load() =>
        JsonSerializer.Deserialize<List<TodoModel>>(File.ReadAllText(_path), Options)
        ?? throw new InvalidDataException("Файл не содержит список задач.");

    public void Save(IEnumerable<TodoModel> tasks) =>
        File.WriteAllText(_path, JsonSerializer.Serialize(tasks, Options));
}

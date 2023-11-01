﻿using Seemon.Todo.Models.Common;

using System.Collections.ObjectModel;

using Task = Seemon.Todo.Models.Task;

namespace Seemon.Todo.Contracts.Services;

public interface ITaskService
{
    event EventHandler<string> Loaded;
    event EventHandler CollectionChanged;

    public bool IsLoaded { get; set; }

    ObservableCollection<Task> ActiveTasks { get; }

    IList<Task> SelectedTasks { get; }

    void LoadTasks(string path);

    void ReloadTasks();

    void AddTask(string raw);

    void UpdateTask(Task current, string raw);

    void DeleteTask(Task task);

    void ToggleCompleted(Task task);

    void ToggleHidden(Task task);

    void ArchiveCompletedTasks();

    void SetPriority(Task task, string priority);

    void SetDate(Task task, string date, DateTypes type);

    void PostponeDate(Task task, string postpone, DateTypes type);

    Task Parse(string raw);
}

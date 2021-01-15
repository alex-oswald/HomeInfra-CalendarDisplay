using FamilyBoard.Data;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyBoard.ViewModels
{
    public interface ITodoViewModel
    {
        string Title { get; }

        List<TodoTask> Items { get; }

        Task InitAsync(Action stateChanged);
    }

    public class TodoViewModel : ITodoViewModel, IDisposable
    {
        private static readonly string _listName = "Family To Do";
        private readonly ITodoManager _todoService;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _stateChanged;

        public TodoViewModel(
            ITodoManager todoService)
        {
            _todoService = todoService;
        }

        public string Title { get; private set; }

        public List<TodoTask> Items { get; private set; } = new();

        public async Task InitAsync(Action stateChanged)
        {
            _stateChanged = stateChanged;
            Title = _listName;
            Items = await _todoService.GetTasksByListNameAsync(_listName);
            _cancellationTokenSource = new CancellationTokenSource();
            _ = PollData(_cancellationTokenSource.Token);
        }

        private async Task PollData(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(120000, cancellationToken);
                Items = await _todoService.GetTasksByListNameAsync(_listName, cancellationToken);
                _stateChanged?.Invoke();
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
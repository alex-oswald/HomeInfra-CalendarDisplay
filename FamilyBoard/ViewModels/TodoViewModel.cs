using FamilyBoard.Data;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyBoard.ViewModels
{
    public interface ITodoViewModel
    {
        string Title { get; }

        Dictionary<string, List<TodoTask>> Lists { get; }

        Task InitAsync(Action stateChanged);
    }

    public class TodoViewModel : ITodoViewModel, IDisposable
    {
        private static readonly string _listName = "Family To Do";
        private readonly TodoListOptions _options;
        private readonly ITodoManager _todoService;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _stateChanged;

        public TodoViewModel(
            IOptions<TodoListOptions> options,
            ITodoManager todoService)
        {
            _options = options.Value;
            _todoService = todoService;
        }

        public string Title { get; private set; }

        public Dictionary<string, List<TodoTask>> Lists { get; private set; } = new();

        public async Task InitAsync(Action stateChanged)
        {
            _stateChanged = stateChanged;
            Title = _listName;
            foreach (var list in _options.TodoLists)
            {
                Lists.Add(list.Name, await _todoService.GetTasksByListNameAsync(list.Name));
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _ = PollData(_cancellationTokenSource.Token);
        }

        private async Task PollData(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(120000, cancellationToken);
                var keys = Lists.Keys;
                // Our grid is only for 4 lists
                foreach (var key in keys.Take(4))
                {
                    Lists[key] = await _todoService.GetTasksByListNameAsync(key);
                }
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
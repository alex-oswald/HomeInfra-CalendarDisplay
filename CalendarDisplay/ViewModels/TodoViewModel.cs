using CalendarDisplay.Data;
using CalendarDisplay.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace CalendarDisplay.ViewModels
{
    public interface ITodoViewModel
    {
        Dictionary<string, List<TodoTask>> Lists { get; }

        Task InitAsync(Action stateChanged);
    }

    public class TodoViewModel : ITodoViewModel, IDisposable
    {
        private readonly TodoListOptions _options;
        private readonly ITodoManager _todoManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _stateChanged;

        public TodoViewModel(
            IOptions<TodoListOptions> options,
            ITodoManager todoManager)
        {
            _options = options.Value;
            _todoManager = todoManager;
        }

        public Dictionary<string, List<TodoTask>> Lists { get; private set; } = new();

        public async Task InitAsync(Action stateChanged)
        {
            _stateChanged = stateChanged;
            foreach (var list in _options.TodoLists)
            {
                Lists.Add(list.Name, await _todoManager.GetTasksByListNameAsync(list.Name));
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _ = PollData(_cancellationTokenSource.Token);
        }

        private async Task PollData(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.UpdateFrequency), cancellationToken);

                var keys = Lists.Keys;
                // Our grid is only for 4 lists
                foreach (var key in keys.Take(4))
                {
                    Lists[key] = await _todoManager.GetTasksByListNameAsync(key);
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
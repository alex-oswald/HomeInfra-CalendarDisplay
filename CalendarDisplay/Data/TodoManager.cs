using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarDisplay.Data
{
    public interface ITodoManager
    {
        Task<List<TodoTask>> GetTasksByListNameAsync(string listName, CancellationToken cancellationToken = default);
    }

    public class TodoManager : ITodoManager
    {
        public List<TodoTaskList> _cachedTodoTaskLists = null;
        private readonly ILogger<CalendarManager> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        public TodoManager(
            ILogger<CalendarManager> logger,
            GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        public async Task<string> GetTodoListIdAsync(string listName, CancellationToken cancellationToken = default)
        {
            // Get our list of toto task lists
            if (_cachedTodoTaskLists == null)
            {
                _cachedTodoTaskLists = (await _graphServiceClient.Me.Todo.Lists
                    .Request()
                    .GetAsync(cancellationToken)).ToList();
                _logger.LogInformation("Todo task list cache was empty, fetched {count} lists", _cachedTodoTaskLists.Count);
            }

            // Get the list we want
            var matchedList = _cachedTodoTaskLists.Where(l => l.DisplayName == listName).SingleOrDefault();
            if (matchedList is null)
            {
                _logger.LogWarning("Unable to find list '{listName}'.", listName);
                throw new Exception($"Unable to find list '{listName}'.");
            }

            return matchedList.Id;
        }

        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        public async Task<List<TodoTask>> GetTasksByListNameAsync(string listName, CancellationToken cancellationToken = default)
        {
            try
            {
                var id = await GetTodoListIdAsync(listName, cancellationToken);

                // Request tasks for our list
                var tasks = (await _graphServiceClient.Me.Todo.Lists[id].Tasks
                    .Request()
                    .GetAsync(cancellationToken)).ToList();

                _logger.LogInformation("{this} success, {eventCount} tasks for {month}.",
                    nameof(GetTasksByListNameAsync), tasks.Count, listName);

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "{this} failed.", nameof(GetTasksByListNameAsync));
                return new();
            }
        }
    }
}
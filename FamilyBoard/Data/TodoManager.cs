using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyBoard.Data
{
    public interface ITodoManager
    {
        Task<List<TodoTask>> GetTasksByListNameAsync(string listName, CancellationToken cancellationToken = default);
    }

    public class TodoManager : ITodoManager
    {
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
        public async Task<List<TodoTask>> GetTasksByListNameAsync(string listName, CancellationToken cancellationToken = default)
        {
            try
            {
                var todoTaskLists = (await _graphServiceClient.Me.Todo.Lists
                    .Request()
                    .GetAsync(cancellationToken)).ToList();

                var matchedList = todoTaskLists.Where(l => l.DisplayName == listName).SingleOrDefault();
                if (matchedList is null)
                {
                    _logger.LogInformation("Unable to find list '{}'.", listName);
                    return new();
                }

                var tasks = (await _graphServiceClient.Me.Todo.Lists[matchedList.Id].Tasks
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
using System.Collections.Generic;

namespace CalendarDisplay.Options
{
    public class TodoListOptions
    {
        public const string Section = "TodoListOptions";

        public List<TodoList> TodoLists { get; set; }

        public int UpdateFrequency { get; set; } = 3_600;

        public class TodoList
        {
            public string Name { get; set; }
        }
    }
}
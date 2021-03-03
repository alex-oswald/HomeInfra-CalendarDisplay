using System.Collections.Generic;

namespace FamilyBoard
{
    public class TodoListOptions
    {
        public List<TodoList> TodoLists { get; set; }

        public class TodoList
        {
            public string Name { get; set; }
        }
    }
}
using System;


namespace ToDoList
{
    
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TaskViewModel
    {
        public string Title { get; set; }
        public string DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public Task OriginalTask { get; set; }
    }
}

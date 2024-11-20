using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private HashSet<int> _notifiedTaskIds = new HashSet<int>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase(); // Create a data base if it does not exist
            TaskDatePicker.SelectedDate = DateTime.Today; // Set the default date to today
            LoadTasks();
            StartTimer();
        }
        

        private TodoContext db = new TodoContext();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskDialog = new AddTaskDialog();
            if (addTaskDialog.ShowDialog() == true) 
            {
                LoadTasks(); // Reload task list after adding a new task
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is TaskViewModel selectedTaskViewModel)
            {
                var editTaskDialog = new EditTaskDialog(selectedTaskViewModel.OriginalTask);
                if (editTaskDialog.ShowDialog() == true)
                {
                    LoadTasks(); // Refresh task list after editing
                }
            }
            else
            {
                MessageBox.Show("Please select a task to edit.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is TaskViewModel selectedTaskViewModel)
            {
                var selectedTask = selectedTaskViewModel.OriginalTask;

                // Deleting a task from the database
                using (var context = new TodoContext())
                {
                    context.Tasks.Remove(selectedTask);
                    context.SaveChanges();
                }

                LoadTasks(); // Refresh task list after deleting
            }
            else
            {
                MessageBox.Show("Please select a task to delete.");
            }
        }

        // Refresh task list
        private void LoadTasks()
        {
            DateTime selectedDate = TaskDatePicker.SelectedDate ?? DateTime.Today;
            using (var context = new TodoContext())
            {
                var tasks = context.Tasks
                    .Where(t => t.DueDate.Date == selectedDate.Date)
                    .Select(t => new TaskViewModel
                    {
                        Title = t.Title,
                        DueDate = t.DueDate.ToString("dd/MM/yyyy HH:mm"),
                        IsCompleted = t.IsCompleted,
                        OriginalTask = t
                    })
                    .ToList();

                TaskListView.ItemsSource = tasks;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskViewModel taskViewModel)
            {
                UpdateTaskCompletionStatus(taskViewModel.OriginalTask, true);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskViewModel taskViewModel)
            {
                UpdateTaskCompletionStatus(taskViewModel.OriginalTask, false);
            }
        }

        private void UpdateTaskCompletionStatus(Task task, bool isCompleted)
        {
            task.IsCompleted = isCompleted;

            using (var context = new TodoContext())
            {
                context.Tasks.Update(task);
                context.SaveChanges();
            }

            LoadTasks(); // Refresh the list for update the view
        }

        private void TaskListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TaskListView.SelectedItem is TaskViewModel selectedTaskViewModel)
            {
                var editTaskDialog = new EditTaskDialog(selectedTaskViewModel.OriginalTask);
                if (editTaskDialog.ShowDialog() == true)
                {
                    LoadTasks(); // Refresh the list for update the view
                }
            }
        }

        // Create the database and table if they don't exist
        private void InitializeDatabase()
        {
            using (var context = new TodoContext())
            {
                context.Database.EnsureCreated(); 
            }
        }

        // Load tasks for the selected date
        private void TaskDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTasks(); 
        }

        //Timer for CheckUpcomingTasks
        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CheckUpcomingTasks();
        }

        //Check if there are any tasks scheduled in the near future
        private void CheckUpcomingTasks()
        {
            DateTime now = DateTime.Now;
            DateTime threshold = now.AddMinutes(10);

            using (var context = new TodoContext())
            {
                var upcomingTasks = context.Tasks
                    .Where(t => t.DueDate > now && t.DueDate <= threshold && !t.IsCompleted)
                    .ToList();

                foreach (var task in upcomingTasks)
                {
                    ShowNotification(task);
                }
            }
        }      

        private void ShowNotification(Task task)
        {
            if (_notifiedTaskIds.Contains(task.Id))
            {
                return; // Already notified about this task
            }

            _notifiedTaskIds.Add(task.Id);

            MessageBox.Show($"Upcoming task: {task.Title} at {task.DueDate.ToString("dd/MM/yyyy HH:mm")}", "Task Reminder", MessageBoxButton.OK);
        }
    }
}

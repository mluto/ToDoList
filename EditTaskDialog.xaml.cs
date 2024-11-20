using System;
using System.Windows;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for EditTaskDialog.xaml
    /// </summary>
    public partial class EditTaskDialog : Window
    {
        private Task _task;

        public EditTaskDialog(Task task)
        {
            InitializeComponent();
            _task = task;

            // Fill fields with data from the task
            TitleTextBox.Text = _task.Title;
            DueDatePicker.SelectedDate = _task.DueDate.Date;
            HourComboBox.SelectedItem = _task.DueDate.Hour.ToString("D2");
            MinuteComboBox.SelectedItem = _task.DueDate.Minute.ToString("D2");
            CompletedCheckBox.IsChecked = _task.IsCompleted;

            // Refill ComboBox of hours
            for (int i = 0; i < 24; i++)
            {
                HourComboBox.Items.Add(i.ToString("D2"));
            }
            HourComboBox.SelectedIndex = _task.DueDate.Hour;

            // Refill ComboBox of minutes
            for (int i = 0; i < 60; i += 5)
            {
                MinuteComboBox.Items.Add(i.ToString("D2"));
            }
            MinuteComboBox.SelectedIndex = _task.DueDate.Minute / 5;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            // Update task
            _task.Title = TitleTextBox.Text;

            if (DueDatePicker.SelectedDate.HasValue)
            {
                _task.DueDate = DueDatePicker.SelectedDate.Value.Date +
                                TimeSpan.FromHours(int.Parse(HourComboBox.SelectedItem.ToString())) +
                                TimeSpan.FromMinutes(int.Parse(MinuteComboBox.SelectedItem.ToString()));
            }
            else
            {
                MessageBox.Show("Please select a valid due date.");
                return;
            }

            // Set completion status
            _task.IsCompleted = CompletedCheckBox.IsChecked ?? false;

            using (var context = new TodoContext())
            {
                context.Tasks.Update(_task);
                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

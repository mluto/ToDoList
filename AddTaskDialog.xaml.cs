using System;
using System.Windows;


namespace ToDoList
{
    /// <summary>
    /// Interaction logic for AddTaskDialog.xaml
    /// </summary>
    public partial class AddTaskDialog : Window
    {
        public AddTaskDialog()
        {
            InitializeComponent();

            // Refill ComboBox of hours
            for (int i = 0; i < 24; i++)
            {
                HourComboBox.Items.Add(i.ToString("D2")); 
            }

            // Set current hours
            HourComboBox.SelectedIndex = DateTime.Now.Hour;

            // Refill ComboBox of minutes
            for (int i = 0; i < 60; i += 5) // Użyj co 5 minut
            {
                MinuteComboBox.Items.Add(i.ToString("D2")); 
            }

            // Set current minutes
            MinuteComboBox.SelectedIndex = DateTime.Now.Minute / 5; 
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            if (HourComboBox.SelectedItem == null || MinuteComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a time for the task.");
                return;
            }

            // Create a new task
            var task = new Task
            {
                Title = TitleTextBox.Text,
                DueDate = DueDatePicker.SelectedDate?.Date +
                          TimeSpan.FromHours(int.Parse(HourComboBox.SelectedItem.ToString())) +
                          TimeSpan.FromMinutes(int.Parse(MinuteComboBox.SelectedItem.ToString())) ?? DateTime.Today,
                IsCompleted = false
            };

            using (var context = new TodoContext())
            {
                context.Tasks.Add(task);
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

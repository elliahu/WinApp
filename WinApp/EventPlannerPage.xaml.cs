using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WinApp.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventPlannerPage : Page
    {
        public ObservableCollection<CustomEvent> Events { get; set; } = new ObservableCollection<CustomEvent>();
        public EventPlannerPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private async void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            DisplayEventDialog();
        }

        private async void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            DisplayEventDialog(eventListView.SelectedItem as CustomEvent);
        }

        private async void DisplayEventDialog(CustomEvent customEvent = null)
        {
            if (customEvent is null)
            {
                customEvent = new CustomEvent();
            }

            // Content of the dilog window
            StackPanel mainPanel = new StackPanel() { Orientation = Orientation.Vertical };   

            TextBox nameBox = new TextBox() { Header = "Event name", Margin = new Thickness(0,0,0,15)};
            nameBox.Text = customEvent.Name;
            mainPanel.Children.Add(nameBox);

            TextBox descBox = new TextBox() { Header = "Event description", Margin = new Thickness(0, 0, 0, 15) };
            descBox.Text = customEvent.Description;
            mainPanel.Children.Add(descBox);

            StackPanel row = new StackPanel() { Orientation = Orientation.Horizontal };
            mainPanel.Children.Add(row);

            CalendarDatePicker startDate = new CalendarDatePicker();
            TimePicker startTime = new TimePicker() { ClockIdentifier = "24HourClock" };
            if (customEvent.StartDate != default(DateTimeOffset))
            {
                startDate.Date = customEvent.StartDate;
                startTime.Time = customEvent.StartDate.TimeOfDay;
            }

            row.Children.Add(startDate);
            row.Children.Add(startTime);

            StackPanel row2 = new StackPanel() { Orientation = Orientation.Horizontal };
            mainPanel.Children.Add(row2);

            CalendarDatePicker endDate = new CalendarDatePicker();
            TimePicker endTime = new TimePicker() { ClockIdentifier = "24HourClock" };
            if (customEvent.EndDate != default(DateTimeOffset))
            {
                endDate.Date = customEvent.EndDate;
                endTime.Time = customEvent.EndDate.TimeOfDay;
            }

            row2.Children.Add(endDate);
            row2.Children.Add(endTime);

            ContentDialog eventDialog = new ContentDialog
            {
                Title = (customEvent.Name is null) ? "Plan new event": "Edit existing event" ,
                Content = mainPanel,
                PrimaryButtonText = (customEvent.Name is null) ? "Create" : "Save",
                CloseButtonText = "Cancel"
            };

            eventDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await eventDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                startDate.Date = startDate.Date.Value.Date.Add(startTime.Time);
                endDate.Date = endDate.Date.Value.Date.Add(endTime.Time);
                var saveEvent = new CustomEvent()
                {
                    EventId = customEvent.EventId,
                    Name = nameBox.Text,
                    Description = descBox.Text,
                    StartDate = startDate.Date.Value,
                    EndDate = endDate.Date.Value,
                };
                await StorageManager.SaveEvent(saveEvent);
                Events.Add(saveEvent);
            }
        }

        private void calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Phase == 0)
            {
                // Register callback for next phase.
                args.RegisterUpdateCallback(calendar_CalendarViewDayItemChanging);
            }
            else if (args.Phase == 1)
            {
                // Register callback for next phase.
                args.RegisterUpdateCallback(calendar_CalendarViewDayItemChanging);
            }
            // Set density bars.
            else if (args.Phase == 2)
            {
                List<Color> densityColors = new List<Color>();
                
                foreach(var e in Events.Where( date => 
                date.StartDate.Date == args.Item.Date.Date ||
                date.EndDate.Date == args.Item.Date.Date ||(
                args.Item.Date.Date > date.StartDate.Date &&
                args.Item.Date.Date < date.EndDate.Date)))
                {
                    densityColors.Add(e.Color);
                }
                args.Item.SetDensityColors(densityColors);
            }
        }

        private async void eventListView_Loaded(object sender, RoutedEventArgs e)
        {
            var customEvents = await StorageManager.LoadAllEvents();
            foreach (var customEvent in customEvents)
            {
                Events.Add(customEvent);
            }
        }

        private async void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            await StorageManager.DeleteEvent((eventListView.SelectedItem as CustomEvent).EventId);
            Events.Remove(eventListView.SelectedItem as CustomEvent);
        }

        private void eventListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(eventListView.SelectedItem is not null)
            {
                calendar.SelectedDates.Clear();
                var day = (eventListView.SelectedItem as CustomEvent).StartDate;
                while (day.Date <= (eventListView.SelectedItem as CustomEvent).EndDate.Date)
                {
                    calendar.SelectedDates.Add(day);
                    day = day.AddDays(1);
                }

                editBtn.IsEnabled = true;
                shareBtn.IsEnabled = true;
                delBtn.IsEnabled = true;
            }
        }
    }
}

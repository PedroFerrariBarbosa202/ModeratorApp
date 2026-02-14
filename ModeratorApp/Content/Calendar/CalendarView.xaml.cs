using System.Globalization;
using TEST_APP.Services;
namespace ModeratorApp.Content.Calendar;

public partial class CalendarView : ContentView
{
	const int Rows = 20;
    const int Column_Length = 6;

    public CalendarView()
	{
		InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e) {
        await InitCalendar();
    }

    private async void OnDateChanged(object sender, DateChangedEventArgs e) { 
        await InitCalendar();
    }

    async Task InitCalendar() {
        // delete old instances of CalendarDays
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        // add loading screen
        Loading loading_page = new Loading();
        var page = GetParentPage() as ActivityPage;
        page.AddLoading();

        int day_counter = 1;
        string current_month = CalendarDatePicker.Date.ToString(
            "MMMM",
            new CultureInfo("pt-BR")
        );

        // get number of days in month
        int year = CalendarDatePicker.Date.Year;
        int month = CalendarDatePicker.Date.Month;

        int days_in_month = DateTime.DaysInMonth(year, month);

        // init month label
        MonthLabel.Text = current_month;

        for (int y = 0; y < Rows; y++) {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int x = 0; x < Column_Length; x++) {
                if (day_counter >= days_in_month) {
                    // remove loading screen
                    page.RemoveLoading();
                    return;
                }

                // check if volunteer was active in speficic day and month
                DateTime time_to_check = new DateTime(year, month, day_counter);

                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // send to CalendarDay class if volunteer was active on day
                CalendarDay calendar_day = new CalendarDay(await IsActiveOnDay(time_to_check));
                calendar_day.ChangeLabel(day_counter);

				CalendarGrid.SetRow(calendar_day, y);
                CalendarGrid.SetColumn(calendar_day, x);

                CalendarGrid.Children.Add(calendar_day);

                // update day counter
                day_counter++;
            }
        }
    }

    async Task<bool> IsActiveOnDay(DateTime time_to_check) {
        var start = time_to_check.Date;
        var end = start.AddDays(1);

        var response = await DatabaseConnector.Client
              .From<Models.Activity>()
              .Where(x => x.created_at >= start && x.created_at < end)
              .Get();

        return response.Models.Count > 0 ? true : false;
    }

    private Page? GetParentPage() {
        Element parent = this;

        while (parent != null) {
            if (parent is Page page)
                return page;

            parent = parent.Parent;
        }

        return null;
    }
}
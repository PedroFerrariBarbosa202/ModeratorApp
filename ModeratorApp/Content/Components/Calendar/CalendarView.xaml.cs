using ModeratorApp.Services;
using System.Diagnostics;
using System.Globalization;
namespace ModeratorApp.Content.Calendar;

public partial class CalendarView : ContentView
{
	const int Rows = 20;
    const int Column_Length = 6;
    int volunteer_id;

    List<Models.Activity> days_active;

    public CalendarView()
	{
        InitializeComponent();
        InitUserPicker();
    }

    async void GetVolunteerId() {
        var volunteer = await DatabaseConnector.Client
            .From<Models.Volunteer>()
            .Where(x => x.name == CalendarUserPicker.SelectedItem)
            .Single();

        if (volunteer != null) 
            volunteer_id = volunteer.volunteer_ID;
    }

    async void InitUserPicker() {
        var response = await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Where(x => x.is_validated == true)
              .Get();
        foreach(var user in response.Models) {
            CalendarUserPicker.Items.Add(user.name);
        }
    }

    private async void UpdateCalendar(object? sender, EventArgs e) {
        await InitCalendar();
    }

    private async Task InitCalendar() {
        GetVolunteerId();

        // stop if user not selected
        if (CalendarUserPicker.SelectedItem == null) 
            return;
        
        // delete old instances of CalendarDays
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        // add loading screen
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

        days_active = await ActiveOnDays(volunteer_id);

        // init month label
        MonthLabel.Text = current_month;

        for (int y = 0; y < Rows; y++) {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int x = 0; x < Column_Length; x++) {
                if (day_counter > days_in_month) {
                    // remove loading screen
                    page.RemoveLoading();
                    return;
                }
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                DateTime time_to_check = new DateTime(year, month, day_counter);
                AddDay(time_to_check, days_active, x, y);

                // update day counter
                day_counter++;
            }
        }
    }

    // adds the calendar day object to grid
    private async void AddDay(DateTime time_to_check, List<Models.Activity> days_active, int x, int y) {
        // send to CalendarDay class if volunteer was active on day
        bool wasActive = days_active.Any(d => d.created_at.Day == time_to_check.Day);
        Debug.WriteLine(wasActive);

        CalendarDay calendar_day = new CalendarDay(volunteer_id, DateOnly.FromDateTime(time_to_check).Day, wasActive);

        CalendarGrid.SetRow(calendar_day, y);
        CalendarGrid.SetColumn(calendar_day, x);

        CalendarGrid.Children.Add(calendar_day);
    }

    // returns a list of the active days a volunteer has, from time begin to time end
    async Task<List<Models.Activity>> ActiveOnDays(int vol_id) {
        List<Models.Activity> days_active = new List<Models.Activity>();

        int year = CalendarDatePicker.Date.Year;
        int month = CalendarDatePicker.Date.Month;

        DateTime startOfMonth = new DateTime(year, month, 1);
        DateTime startOfNextMonth = startOfMonth.AddMonths(1);

        var response = await DatabaseConnector.Client
            .From<Models.Activity>()
            .Where(x => x.volunteer_ID == vol_id)
            .Where(x => x.hours_diff != null)
            .Where(x => x.finished_at >= startOfMonth)
            .Where(x => x.finished_at < startOfNextMonth)
            .Get();

        // populate list
        foreach (var day in response.Models) {
            days_active.Add(day);
        }

        return days_active;
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

    private void CreateReport(object? sender, EventArgs e) {
        // dont create report if calendar not yet created
        if (days_active == null)
            return;

        days_active = days_active
                .OrderBy(x => x.created_at)
                .ToList();

        // create pdf
        var report = new ReportDocument();
        report.SetTitle("Relatório de atividade do voluntário");

        report.AddContent(new ContentCommand {
            type = ContentType.Spacing,
            magnitude = 16,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.Text,
            content = $"Nome: {CalendarUserPicker.SelectedItem}",
            font_size = 18,
            bold = true,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.Spacing,
            magnitude = 6,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.Text,
            content = $"Mês selecionado: {CalendarDatePicker.Date.Month}/{CalendarDatePicker.Date.Year}",
            font_size = 18,
            bold = true,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.Text,
            content = $"Dias de Atividade no Mês: {days_active.Count}",
            font_size = 16,
            bold = true,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.LineHorizontal,
            thickness = 2,
        });

        report.AddContent(new ContentCommand {
            type = ContentType.Spacing,
            magnitude = 5,
        });

        foreach (var day in days_active) {
            // get the time that the volunteer was active
            string formated_time = $"{(int)day.hours_diff}h";

            report.AddContent(new ContentCommand {
                type = ContentType.Text,
                content = $"Dia ativo: {DateOnly.FromDateTime(day.created_at).ToString()}  |  Horário de Início: {day.created_at.ToString("HH:mm:ss")}  |  Tempo Decorrido: {formated_time}",
                font_size = 13,
                bold = true,
                allign_center = true
            });
        }
        report.Compose();
    }
}
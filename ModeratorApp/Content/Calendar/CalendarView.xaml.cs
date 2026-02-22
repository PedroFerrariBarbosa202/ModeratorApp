using Microcharts;
using ModeratorApp.Models;
using SkiaSharp;
using System;
using System.Globalization;
using TEST_APP.Services;
namespace ModeratorApp.Content.Calendar;

public partial class CalendarView : ContentView
{
	const int Rows = 20;
    const int Column_Length = 6;

    List<(DateTime, DateTime)>? _days_active = null;

    public CalendarView()
	{
		InitializeComponent();
        InitUserPicker();
    }
    async void CreateChart() {
        var entries = new List<ChartEntry>();

        var volunteer = await DatabaseConnector.Client
            .From<Models.Volunteer>()
            .Where(x => x.name == CalendarUserPicker.SelectedItem)
            .Single();

        int year = CalendarDatePicker.Date.Year;
        int month = CalendarDatePicker.Date.Month;

        int daysInMonth = DateTime.DaysInMonth(year, month);

        List<(DateTime start, DateTime end)> days_active =
            await ActiveOnDays(
                (new DateTime(year, month, 1),
                 new DateTime(year, month, daysInMonth)),
                volunteer.volunteer_ID);

        for (int day = 1; day <= daysInMonth; day++) {
            DateTime dayStart = new DateTime(year, month, day, 0, 0, 0);
            DateTime dayEnd = new DateTime(year, month, day, 23, 59, 59);

            bool hasActivity = days_active.Any(activity =>
                activity.start < dayEnd &&
                activity.end > dayStart);

            if (hasActivity) {
                DateTime currentDay = new DateTime(year, month, day);

                var activity = days_active
                    .FirstOrDefault(a => a.start.Date == currentDay.Date);

                TimeSpan time = activity.end - activity.start;
                float hours = (float)time.TotalHours;
                float rounded = (float)Math.Round(hours, 1);

                entries.Add(new ChartEntry(hours) {
                    Label = $"{day}",
                    ValueLabel = $"{rounded}h",
                    Color = SKColor.Parse("#3498db")
                });
            }
            else {
                entries.Add(new ChartEntry(0) {
                    Label = $"{day}",
                    ValueLabel = $"{0}h",
                    Color = SKColor.Parse("#3498db")
                });
            }
        }

        // create chart
        ActivityChart.Chart = new LineChart {
            Entries = entries,
            BackgroundColor = SKColors.White,
        };
    }


    async void InitUserPicker() {
        var response = await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Get();
        foreach(var user in response.Models) {
            CalendarUserPicker.Items.Add(user.name);
        }
    }

    private async void UpdateCalendar(object? sender, EventArgs e) {
        await InitCalendar();
    }

    private async Task InitCalendar() {
        CreateChart();

        // stop if user not selected
        if (CalendarUserPicker.SelectedItem == null) 
            return;
        
        // delete old instances of CalendarDays
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        // add loading screen
        Loading loading_page = new Loading();
        var page = GetParentPage() as ActivityPage;
        page.AddLoading();

        // get id from user selected
        var volunteer = await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Where(x => x.name == CalendarUserPicker.SelectedItem)
              .Single();

        int day_counter = 1;
        string current_month = CalendarDatePicker.Date.ToString(
            "MMMM",
            new CultureInfo("pt-BR")
        );

        // get number of days in month
        int year = CalendarDatePicker.Date.Year;
        int month = CalendarDatePicker.Date.Month;

        int days_in_month = DateTime.DaysInMonth(year, month);

        // create list of days that volunteer is active
        List<(DateTime, DateTime)> days_active = await ActiveOnDays((new DateTime(year, month, 1), new DateTime(year, month, 1)), volunteer.volunteer_ID);
        _days_active = days_active;

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
                AddDay(time_to_check, days_active, x, y, day_counter);

                // update day counter
                day_counter++;
            }
        }
    }

    // adds the calendar day object to grid
    private void AddDay(DateTime time_to_check, List<(DateTime, DateTime)> days_active, int x, int y, int day_counter) {

        // send to CalendarDay class if volunteer was active on day
        bool wasActive = days_active.Any(d => d.Item1.Date == time_to_check.Date);

        CalendarDay calendar_day = new CalendarDay(wasActive);
        calendar_day.ChangeLabel(day_counter);

        CalendarGrid.SetRow(calendar_day, y);
        CalendarGrid.SetColumn(calendar_day, x);

        CalendarGrid.Children.Add(calendar_day);
    }

    // returns a list of the active days a volunteer has, from time begin to time end
    async Task<List<(DateTime, DateTime)>> ActiveOnDays((DateTime, DateTime) time_to_check, int vol_id) {
        List<(DateTime, DateTime)> days_active = new List<(DateTime, DateTime)>();

        var response = await DatabaseConnector.Client
              .From<Models.Activity>()
              .Where(x => x.volunteer_ID == vol_id)
              .Get();

        // populate list
        foreach(var day in response.Models) {
            if (day.created_at.Month != time_to_check.Item1.Month)
                continue;
            days_active.Add((day.created_at, day.finished_at));
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
        if (_days_active == null)
            return;

        _days_active.Sort();

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
            content = $"Dias de Atividade no Mês: {_days_active.Count}",
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

        foreach (var day in _days_active) {
            // get the time that the volunteer was active
            TimeSpan tempo = day.Item2 - day.Item1;
            string formated_time = $"{(int)tempo.TotalHours}h {tempo.Minutes}min";

            report.AddContent(new ContentCommand {
                type = ContentType.Text,
                content = $"Dia ativo: {DateOnly.FromDateTime(day.Item1).ToString()}  |  Horário de Início: {day.Item1.ToString("HH:mm:ss")}  |  Tempo Decorrido: {formated_time}",
                font_size = 13,
                bold = true,
                allign_center = true
            });
        }
        report.Compose();
    }

    // activate and deactivate graph
    private void OnGraphicShow(object sender, TappedEventArgs e) {
        ActivityChart.IsVisible = ActivityChart.IsVisible ? false : true;
        GraphShowLabel.Text = ActivityChart.IsVisible ? "Esconder Gráfico" : "Mostrar Gráfico";
    }
}
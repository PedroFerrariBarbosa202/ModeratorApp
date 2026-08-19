using ModeratorApp.Services;
using System.Diagnostics;

namespace ModeratorApp.Content.Calendar;

public partial class CalendarDay : ContentView
{
    int day;
    int volunteer_id;
	public CalendarDay(int _volunteer_id, int _day, bool is_active)
	{
        volunteer_id = _volunteer_id;
        day = _day;
		InitializeComponent();
        ChangeLabel();

        if (is_active) {
			DayBorder.BackgroundColor = Colors.Green;
        }
	}

    public void ChangeLabel() {
		DayLabel.Text = day.ToString();
		return;
    }
}
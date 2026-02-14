namespace ModeratorApp.Content.Calendar;

public partial class CalendarDay : ContentView
{
	public CalendarDay(bool is_active)
	{
		InitializeComponent();

		if (is_active) {
			DayBorder.BackgroundColor = Colors.Green;
        }
	}

	public void ChangeLabel(int day) {
		DayLabel.Text = day.ToString();
		return;
    }
}
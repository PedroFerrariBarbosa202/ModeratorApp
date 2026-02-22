using Microcharts;
using ModeratorApp.Content;
using SkiaSharp;

namespace ModeratorApp;

public partial class ActivityPage : ContentPage
{
    Loading current_loading;
	public ActivityPage()
	{
		InitializeComponent();
    }

	public void AddLoading() {
        Loading loading_page = new Loading();
        current_loading = loading_page;

        ContentGrid.Children.Add(loading_page);
    }

    public void RemoveLoading() {
        if (current_loading != null) {
            ContentGrid.Children.Remove(current_loading);
        }
    }
}
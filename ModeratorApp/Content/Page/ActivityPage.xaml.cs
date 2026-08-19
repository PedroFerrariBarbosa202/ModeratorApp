using ModeratorApp.Services;
namespace ModeratorApp;

public partial class ActivityPage : ContentPage
{
	public ActivityPage()
	{
		InitializeComponent();
    }

	public void AddLoading() {
        OverlayManager.SetLoadingOverlay(ContentGrid);
    }

    public void RemoveLoading() {
        OverlayManager.RemoveLoadingOverlay(ContentGrid);
    }
}
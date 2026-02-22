
namespace ModeratorApp.Content.View;

public partial class Tag : ContentView
{
	Models.Sector data;

    public Tag(Models.Sector _data)
	{
		data = _data;
		InitializeComponent();

        NameTag.Text = data.name;
        Border.Stroke = Color.FromArgb(data.color);
    }
}
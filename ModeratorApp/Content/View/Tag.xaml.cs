using ModeratorApp.Services;

namespace ModeratorApp.Content.View;

public partial class Tag : ContentView
{
	CardManager.TagData data;

    public Tag(CardManager.TagData _data)
	{
		data = _data;
		InitializeComponent();

        NameTag.Text = data.name;
        Border.Stroke = data.color;
    }
}
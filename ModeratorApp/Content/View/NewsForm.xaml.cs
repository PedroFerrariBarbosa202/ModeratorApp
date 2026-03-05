using ModeratorApp.Models;
using ModeratorApp.Services;
namespace ModeratorApp.Content.View;

public partial class NewsForm : ContentView
{
    int? id;
	public NewsForm(int? _id)
	{
		InitializeComponent();
        id = _id;
	}

	private async void OnAddNewsClicked(object sender, EventArgs e) {
		var image_byte = await ImageService.ImageSourceToBytes(ImageChooser.Source);


        News news = new News {
			title = TitleEntry.Text,
			content = ContentEntry.Text,
			image = Convert.ToBase64String(image_byte),
            created_at = DateTime.Now,
        };

        if (!id.HasValue) {
            await DatabaseConnector.Client
                .From<Models.News>()
                .Insert(news);
        }
        else { 
            news.id = id.Value;

            await DatabaseConnector.Client
                .From<Models.News>()
                .Where(x => x.id == id.Value)
                .Update(news);
        }


        // if everything works, close tab and re-init page
        NewsPage? page = GetParentPage() as NewsPage;

        if(page != null) 
            page.InitNewsCards(null, null);

        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }

    private async void OnPickImageClicked(object sender, EventArgs e) {
        try {
            var result = await FilePicker.PickAsync(new PickOptions {
                PickerTitle = "Selecione uma imagem",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null) {
                byte[] user_img = await ImageService.FileResultToBytesAsync(result);
                ImageChooser.Source = ImageSource.FromFile(result.FullPath);
            }
        }
        catch (Exception) {
            //TODO!
        }
    }

    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
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
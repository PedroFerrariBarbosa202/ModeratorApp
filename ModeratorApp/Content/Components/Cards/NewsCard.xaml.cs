using ModeratorApp.Models;
using ModeratorApp.Services;

namespace ModeratorApp.Cards;

public partial class NewsCard : ContentView
{
	News news =	new News();
	public NewsCard(News _news)
	{
		InitializeComponent();
		news = _news;

		byte[] img_byte = Convert.FromBase64String(_news.image);
		var img = ImageService.BytesToImageSource(img_byte);

        TitleEntry.Text = _news.title;
        ContentEntry.Text = _news.content;
		ImageView.Source = img;
    }

    private void OnEditNewsClicked(object sender, EventArgs e) {
        NewsPage? page = GetParentPage() as NewsPage;

        if (page != null) 
            page.AddNewsForm(news.id);
        
	}

    private async void OnRemoveNewsClicked(object sender, EventArgs e) {
        await DatabaseConnector.Client
                    .From<Models.News>()
                    .Where(v => v.id == news.id)
                    .Delete();

        NewsPage? page = GetParentPage() as NewsPage;

        if(page != null)
            page.InitNewsCards(null, null);
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
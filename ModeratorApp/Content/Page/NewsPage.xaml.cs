using ModeratorApp.Content;
using ModeratorApp.Cards;
using ModeratorApp.Content.View;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp;

public partial class NewsPage : ContentPage
{
	public NewsPage()
	{
		InitializeComponent();
		InitNewsCards(null, null);
	}

	public async void InitNewsCards(object? sender, EventArgs? e) {
		Loading loading_page = new Loading();
		ContentGrid.Children.Add(loading_page);

		NewsStack.Children.Clear();

        var response = await DatabaseConnector.Client
			 .From<Models.News>()
			 .Get();

		foreach (var item in response.Models) {
			Models.News news = new Models.News {
				id = item.id,
				title = item.title,
				content = item.content,
				image = item.image,
				created_at = item.created_at,
			};
			NewsCard card = new NewsCard(news);
            NewsStack.Children.Add(card);
		}

        ContentGrid.Children.Remove(loading_page);
    }

	private void OnOpenNewsFormClicked(object sender, EventArgs e) {
		AddNewsForm(null);
	}

	public void AddNewsForm(int? id) {
        NewsForm news_form = new NewsForm(id);
        ContentGrid.Children.Add(news_form);
    }
}
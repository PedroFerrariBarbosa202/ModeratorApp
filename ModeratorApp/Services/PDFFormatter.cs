using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public enum ContentType {
    Text,
    LineHorizontal,
    Spacing,
}

public class ContentCommand() {
    public ContentType type;

    // text
    public string content = "";
    public bool bold;
    public bool allign_center;
    public int font_size;

    // line horizontal
    public int thickness;

    //spacing
    public int magnitude;
}

public class ReportDocument{

    string Title = "";
    List<ContentCommand> Content = new List<ContentCommand>();

    public void SetTitle(string _title) {
        Title = _title;
        return;
    }
    public void AddContent(ContentCommand cont) {
        Content.Add(cont);
        return;
    }

    public async void Compose() {
        QuestPDF.Settings.License = LicenseType.Community;

        var filePath = Path.Combine(FileSystem.AppDataDirectory, "report.pdf");

        Document.Create(container => {
            container.Page(page => {
                page.Margin(30);

                page.Header()
                    .Text(Title)
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col => {
                    foreach (ContentCommand cmd in Content) {
                        col.Spacing(10);
                        switch (cmd.type) {
                            case ContentType.Text:

                            var item = col.Item();

                            if (cmd.allign_center)
                                item = item.AlignCenter();

                            item.Text(text => {
                                var span = text.Span(cmd.content);

                                if (cmd.bold)
                                    span.Bold();

                                span.FontSize(cmd.font_size);
                            });
                            break;

                            case ContentType.LineHorizontal:
                            col.Item().LineHorizontal(cmd.thickness);
                            break;

                            case ContentType.Spacing:
                            col.Item().Height(cmd.magnitude);
                            break;
                        }
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x => {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf(filePath);

        await Launcher.Default.OpenAsync(new OpenFileRequest {
            File = new ReadOnlyFile(filePath)
        });
    }
}


using ModeratorApp.Services;

namespace ModeratorApp;

public partial class LoginPage : ContentPage {
    public LoginPage() {
        InitializeComponent();
    }

    private async void Submited(object sender, EventArgs e) {
        OverlayManager.SetLoadingOverlay(RootGrid);

        string email = Login.Text?.Trim() ?? "";
        string password = Password.Text ?? "";

        try {
            // init database
            await DatabaseConnector.InitializeAsync();

            var session = await DatabaseConnector.Client!.Auth
                .SignIn(email, password);

            if (session?.User != null) {
                string uid = session.User.Id;

                // UID of the authenticated user
                Console.WriteLine($"Logged in UID: {uid}");

                await Shell.Current.GoToAsync(
                    $"//EntrancePage?uid={uid}"
                );
            }
            else {
                ShowLoginError("Email ou senha incorretos. Tente novamente.");
            }
            OverlayManager.RemoveLoadingOverlay(RootGrid);
        }
        catch (Exception ex) {
            ShowLoginError(ex.Message);
            OverlayManager.RemoveLoadingOverlay(RootGrid);
        }
    }

    private void ShowLoginError(String erroMessage) {
        OverlayManager.SetWarningOverlay(
            new WarningOverlayData {
                Title = "Erro no Login",
                Message = erroMessage
            },
            RootGrid
        );
    }
}
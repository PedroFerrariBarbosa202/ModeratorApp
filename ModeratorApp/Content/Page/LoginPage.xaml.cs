namespace ModeratorApp;
public partial class LoginPage : ContentPage {
    public LoginPage() {
        InitializeComponent();
    }

    private async void Submited(object sender, EventArgs e) {
        string log = Login.Text;
        string passw = Password.Text;

        if (log == "amo" && passw == "123") {
            await Shell.Current.GoToAsync($"//EntrancePage");

        }
        else {
            await DisplayAlert("Login Falhou", "Coloque um login ou senha válido", "Tentar novamente");
        }
    }
}

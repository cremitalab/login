using fase1.Service;

namespace fase1.Views;

public partial class HomePages : ContentPage
{
    private readonly string uid;

    public HomePages(string uid)
    {
        InitializeComponent();
        this.uid = uid;

        CargarInfoUsuario();
    }

    private async void CargarInfoUsuario()
    {
        // Más adelante: usar Firebase Firestore o Realtime DB
        nombreLabel.Text = $"UID actual: {uid}";
    }
    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        try
        {
            // Cerrar sesión en Firebase
            FirebaseAuthService.CerrarSesion(); // Asumiendo que tu clase se llama FirebaseService o similar

            // Regresar a la pantalla de login
            await Navigation.PushAsync(new loginPages());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
        }
    }


}

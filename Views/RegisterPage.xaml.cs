using System.Threading.Tasks;
using fase1.Service;
using fase1.Utils;
namespace fase1.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
    private async void ButtonCrearCuenta(object sender, EventArgs e)
    {
        var nombre = nombreEntry.Text;
        var email = correoEntry.Text;
        var password = passwordEntry.Text;
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
            return;
        }

        if (!email.Contains("@") || !email.Contains("."))
        {
            await DisplayAlert("Error", "Correo electrónico inválido.", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres.", "OK");
            return;
        }

        try
        {

            var auth = new FirebaseAuthService();
            var cred = await auth.CrearUsuario(email, password, nombre);
            await DisplayAlert("Registro exitoso", $"Bienvenido {nombre}", "OK");
            await Navigation.PushAsync(new HomePages(cred.User.Uid));
        }
        catch (FirebaseAuthFormattedException ex)
        {
            string mensaje = FirebaseErrorHelper.InterpretarErrorFirebase(ex);

            if (mensaje == "El correo electrónico ya está en uso.")
            {
                await DisplayAlert("Correo duplicado", mensaje, "OK");
                return;
            }

            await DisplayAlert("Error al crear cuenta", mensaje, "OK");
        }
        catch (Exception ex)
        {
            string mensaje = FirebaseErrorHelper.InterpretarErrorFirebase(ex);
            await DisplayAlert("Error al crear cuenta", mensaje, "OK");
        }



    }
    // En RegisterPage.xaml.cs
    private async void OnTapLogin(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new loginPages()); ; // O Navigation.PushAsync(new loginPages());
    }

}

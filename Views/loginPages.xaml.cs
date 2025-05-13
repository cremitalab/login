using System.Threading.Tasks;
using fase1.Service;
using fase1.Utils;


namespace fase1.Views;

public partial class loginPages : ContentPage
{
    public loginPages()
    {
        InitializeComponent();
  

    }

    private async void ButtonLogin(object sender, EventArgs e)
    {
        // Verificar si el usuario está bloqueado
        if (Preferences.ContainsKey("bloqueo_temporal"))
        {
            DateTime tiempoBloqueo = Preferences.Get("bloqueo_temporal", DateTime.MinValue);
            TimeSpan tiempoPasado = DateTime.Now - tiempoBloqueo;

            if (tiempoPasado.TotalSeconds < 30)
            {
                int segundosRestantes = 30 - (int)tiempoPasado.TotalSeconds;
                await DisplayAlert("Bloqueado temporalmente", $"Has sido bloqueado por intentos fallidos.\nIntenta nuevamente en {segundosRestantes} segundos.", "OK");
                return;
            }
            else
            {
                // Pasaron los 30 segundos, desbloquear y reiniciar contador
                Preferences.Remove("bloqueo_temporal");
                Preferences.Set("intentos_fallidos", 0);
            }
        }

        string email = correoEntry.Text;
        string password = pswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Correo y contraseña son obligatorios.", "OK");
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
            var firebaseAuthService = new FirebaseAuthService();
            var credenciales = await firebaseAuthService.CargarUsuario(email, password);

            // Éxito: limpiar intentos y bloqueo
            Preferences.Set("intentos_fallidos", 0);
            Preferences.Remove("bloqueo_temporal");

            Preferences.Set("uid", credenciales.User.Uid);
            await Navigation.PushAsync(new HomePages(credenciales.User.Uid));
        }
        catch (FirebaseAuthFormattedException ex)
        {
            string mensaje = FirebaseErrorHelper.InterpretarErrorFirebase(ex);

            if (mensaje == "La contraseña es incorrecta." || mensaje == "Correo o contraseña incorrectos.")
            {
                int intentos = Preferences.Get("intentos_fallidos", 0) + 1;
                Preferences.Set("intentos_fallidos", intentos);

                if (intentos >= 3)
                {
                    Preferences.Set("bloqueo_temporal", DateTime.Now);
                    await DisplayAlert("Demasiados intentos", $"Has fallado {intentos} veces.\nTu acceso estará bloqueado por 30 segundos.", "OK");
                    return;
                }
                else
                {
                    int intentosRestantes = 3 - intentos;
                    await DisplayAlert("Intento fallido", $"{mensaje}\nIntentos restantes: {intentosRestantes}", "OK");
                    return;
                }
            }

            await DisplayAlert("Error al iniciar sesión", mensaje, "OK");
        }
        catch (Exception ex)
        {
            string mensaje = FirebaseErrorHelper.InterpretarErrorFirebase(ex);
            await DisplayAlert("Error al iniciar sesión", mensaje, "OK");
        }
    }





    // En loginPages.xaml.cs
    private async void OnTapRegistro(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void ButtonGoogleLogin(object sender, EventArgs e)
    {

    }
    private async void OnTapcontraseña(object sender, EventArgs e)
    {
        string email = correoEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Por favor, ingresa tu correo para recuperar la contraseña.", "OK");
            return;
        }

        try
        {
            await FirebaseAuthService.RecuperarContraseña(email);
            await DisplayAlert("Éxito", "Se ha enviado un correo para restablecer tu contraseña.", "OK");
        }
        catch (Exception ex)
        {
            string mensaje = FirebaseErrorHelper.InterpretarErrorFirebase(ex);
            await DisplayAlert("Error", mensaje, "OK");
        }
    }

}

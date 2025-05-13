    namespace fase1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // ✅ Aquí revisas si ya hay UID guardado
            string uidGuardado = Preferences.Get("uid", null);

            if (!string.IsNullOrEmpty(uidGuardado))
            {
                // Redirige a la Home directamente
                return new Window(new NavigationPage(new Views.HomePages(uidGuardado)));
            }

            // Si no hay sesión, va al login
            return new Window(new NavigationPage(new Views.loginPages()));
        }
    }
}


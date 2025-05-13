using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Firebase.Auth.Repository;
using System.Text.Json;
using fase1.Utils;


namespace fase1.Service
{
    public class FirebaseAuthService
    {
        public static FirebaseAuthClient ConectarFirebase()
        {
            var config = LeerConfig();
            return new FirebaseAuthClient(config);
        }




        public async Task<UserCredential> CargarUsuario(string Email, string Password)
        {
            var cliente = ConectarFirebase();

            try
            {
                var userCredential = await cliente.SignInWithEmailAndPasswordAsync(Email, Password);
                return userCredential;
            }
            catch (FirebaseAuthHttpException fahe)
            {
                var json = fahe.ResponseData;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(json);
                    string code = doc.RootElement.GetProperty("error").GetProperty("message").GetString();

                    // Aquí ya no lanzamos excepción, devolvemos mensaje legible
                    throw new FirebaseAuthFormattedException(code, json); // <- usar excepción personalizada
                }
                catch
                {
                    throw new Exception("FIREBASE_UNKNOWN_JSON: " + json);
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Error general de autenticación: " + ex.Message);
            }
        }

        public async Task<UserCredential> CrearUsuario(string Email, string Password, string Nombre)
        {
            var cliente = ConectarFirebase();
            try
            {
                var userCredential = await cliente.CreateUserWithEmailAndPasswordAsync(Email, Password, Nombre);
                return userCredential;
            }
            catch (FirebaseAuthHttpException fahe)
            {
                var json = fahe.ResponseData;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(json);
                    string code = doc.RootElement.GetProperty("error").GetProperty("message").GetString();
                    throw new FirebaseAuthFormattedException(code, json);
                }
                catch
                {
                    throw new Exception("FIREBASE_UNKNOWN_JSON: " + json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error general de autenticación: " + ex.Message);
            }
        }

        public static void CerrarSesion()
        {
            try
            {
                var cliente = ConectarFirebase();
                if (cliente == null)
                {
                    Console.WriteLine("⚠️ Cliente Firebase es null.");
                    return;
                }

                cliente.SignOut();
                Console.WriteLine("✅ Sesión cerrada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al cerrar sesión: " + ex.Message);
            }
        }


        public static async Task RecuperarContraseña(string correo)
        {
            try
            {
                var cliente = ConectarFirebase();
                if (cliente == null)
                    throw new Exception("No se pudo conectar con Firebase. Revisa la configuración.");

                await cliente.ResetEmailPasswordAsync(correo);
            }
            catch (FirebaseAuthHttpException fahe)
            {
                var json = fahe.ResponseData;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(json);
                    string code = doc.RootElement.GetProperty("error").GetProperty("message").GetString();

                    // Lanzamos la excepción personalizada para que sea manejada en la vista
                    throw new FirebaseAuthFormattedException(code, json);
                }
                catch (Exception parseEx)
                {
                    // Si el JSON no puede ser procesado, lanzamos una excepción general
                    throw new Exception("FIREBASE_UNKNOWN_JSON: " + json + " Error: " + parseEx.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error general al recuperar contraseña: " + ex.Message);
            }
        }




        public static FirebaseAuthConfig LeerConfig()
        {
            try
            {
                var assembly = typeof(FirebaseAuthService).Assembly;
                var resourceName = "fase1.Resources.Config.config.json";

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new FileNotFoundException($"❌ No se encontró el recurso embebido: {resourceName}");

                using StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();

                var configData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                var firebaseSettings = configData["Firebase"];

                return new FirebaseAuthConfig
                {
                    ApiKey = firebaseSettings["ApiKey"],
                    AuthDomain = firebaseSettings["AuthDomain"],
                    Providers = new FirebaseAuthProvider[]
                    {
                new EmailProvider(),
                new GoogleProvider().AddScopes("email")
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al leer config: " + ex.Message);
                return null;
            }
        }


    }
}

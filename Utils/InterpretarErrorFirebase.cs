using System;
using System.Text.Json;
using Firebase.Auth;

namespace fase1.Utils
{
    public static class FirebaseErrorHelper
    {
        // Método para interpretar los errores de Firebase
        public static string InterpretarErrorFirebase(Exception ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;

            if (mensaje.StartsWith("INVALID_") || mensaje.StartsWith("EMAIL_") || mensaje.StartsWith("USER_"))
                return TraducirCodigoFirebase(mensaje);

            if (mensaje.Contains("EMAIL_NOT_FOUND"))
                return "El correo no está registrado.";
            if (mensaje.Contains("INVALID_PASSWORD"))
                return "La contraseña es incorrecta.";
            if (mensaje.Contains("USER_DISABLED"))
                return "La cuenta ha sido deshabilitada.";
            if (mensaje.Contains("INVALID_EMAIL"))
                return "Correo electrónico inválido.";
            if (mensaje.Contains("EMAIL_EXISTS"))
                return "Este correo ya está registrado.";
            if (mensaje.Contains("INVALID_LOGIN_CREDENTIALS"))
                return "Correo o contraseña incorrectos.";
            if (mensaje.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                return "Demasiados intentos. Intenta nuevamente más tarde.";

            // Si es una excepción específica de FirebaseAuth
            if (ex is FirebaseAuthHttpException firebaseEx)
            {
                var json = firebaseEx.ResponseData;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("error", out var errorElement) &&
                        errorElement.TryGetProperty("message", out var messageElement))
                    {
                        string code = messageElement.GetString();
                        return TraducirCodigoFirebase(code);
                    }
                }
                catch
                {
                    // ignorar parse error
                }

                // Si no pudimos parsear, intentar extraer código manualmente desde el JSON en texto plano
                if (json.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                    return "Demasiados intentos. Intenta nuevamente más tarde.";
                if (json.Contains("INVALID_PASSWORD"))
                    return "La contraseña es incorrecta.";
                if (json.Contains("EMAIL_NOT_FOUND"))
                    return "El correo no está registrado.";
                if (json.Contains("EMAIL_EXISTS"))
                    return "Este correo ya está registrado.";
            }

            return "Error inesperado: " + mensaje;
        }


        // Traducir los códigos de error específicos de Firebase
        private static string TraducirCodigoFirebase(string code)
        {
            return code switch
            {
                "EMAIL_NOT_FOUND" => "El correo no está registrado.",
                "INVALID_PASSWORD" => "La contraseña es incorrecta.",
                "USER_DISABLED" => "La cuenta ha sido deshabilitada.",
                "INVALID_EMAIL" => "Correo electrónico inválido.",
                "EMAIL_EXISTS" => "Este correo ya está registrado.",
                "INVALID_LOGIN_CREDENTIALS" => "Correo o contraseña incorrectos.",
                "TOO_MANY_ATTEMPTS_TRY_LATER" => "Demasiados intentos. Intenta nuevamente más tarde.",
                "WEAK_PASSWORD" => "La contraseña es demasiado débil.",
                "MISSING_EMAIL" => "El correo es obligatorio.",
                "MISSING_PASSWORD" => "La contraseña es obligatoria.",
                "EMAIL_ALREADY_IN_USE" => "Este correo ya está en uso por otra cuenta.",
                "INVALID_OAUTH_PROVIDER" => "Proveedor de OAuth inválido.",
                "INVALID_ID_TOKEN" => "Token de identificación inválido.",
                "USER_NOT_FOUND" => "Usuario no encontrado.",
                "EXPIRED_OAUTH_CREDENTIALS" => "Las credenciales de OAuth han expirado.",
                "ACCOUNT_EXISTS_WITH_DIFFERENT_CREDENTIAL" => "Ya existe una cuenta con estas credenciales.",
                "CREDENTIAL_ALREADY_IN_USE" => "Las credenciales ya están en uso.",
                "OPERATION_NOT_ALLOWED" => "La operación no está permitida.",
                "UNEXPECTED_ERROR" => "Error inesperado. Intenta de nuevo.",
                "EMAIL_NOT_VERIFIED" => "El correo electrónico no está verificado.",
                "USER_TOKEN_EXPIRED" => "El token de usuario ha expirado.",
                "USER_MISMATCH" => "No hay coincidencia entre el usuario y la contraseña.",
                "INVALID_API_KEY" => "La clave de API es inválida.",
                _ => "Error de Firebase: " + code
            };
        }

    }
}

using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace xamarinrest.Configuration
{
    public class Prefs
    {
        private static ISettings AppSettings => CrossSettings.Current;
        private static string filePrefs => "app.prefs";

        //Token do dispositivo
        public static string DeviceToken
        {
            get => AppSettings.GetValueOrDefault( nameof( DeviceToken ), string.Empty );
            set => AppSettings.AddOrUpdateValue( nameof( DeviceToken ), value );
        }

        //Método genérico para obter o valor de uma key salva nas Prefs
        public static string getString( string key )
        {
            return AppSettings.GetValueOrDefault( key, string.Empty, filePrefs);
        }

        //Método genérico para definir o valor de uma key salva nas Prefs
        public static void putString( string key, string value )
        {
            AppSettings.AddOrUpdateValue( key, value, filePrefs);
        }

        //Método genérico para obter o valor de uma DateTime de uma key salva nas Prefs
        public static DateTime getDateTime( string key )
        {
            return AppSettings.GetValueOrDefault( key, new DateTime( 1971, 1, 1 ), filePrefs );
        }

        //Método genérico para definir o valor tipo DateTime de uma key salva nas Prefs
        public static void setDateTime( string key, DateTime value )
        {
            AppSettings.AddOrUpdateValue(key, value, filePrefs);
        }
    }
}

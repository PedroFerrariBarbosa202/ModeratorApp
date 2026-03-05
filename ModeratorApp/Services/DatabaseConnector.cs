using Supabase;
using Microsoft.Maui.Networking;

namespace ModeratorApp.Services {
    public static class DatabaseConnector {
        private static readonly string url =
            "https://xacemhnbvvfqpzzbcwei.supabase.co";

        private static readonly string anonKey =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InhhY2VtaG5idnZmcXB6emJjd2VpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjcxOTQ2NTgsImV4cCI6MjA4Mjc3MDY1OH0.NqOQz5h6snNDoHOFSdL15CciHjWtUCp2gu7yXaucaE4";

        public static Client Client;

        public static async Task InitializeAsync() {
            if (Client != null)
                return;

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) {
                throw new InvalidOperationException(
                    "Sem conexão com a internet. Verifique sua rede antes de continuar.");
            }

            try {
                var options = new SupabaseOptions {
                    AutoConnectRealtime = false
                };

                Client = new Client(url, anonKey, options);

                await Client.InitializeAsync();
            }
            catch (Exception ex) {
                Client = null; 
                throw new Exception("Erro ao conectar com o servidor Supabase.", ex);
            }
        }
    }
}
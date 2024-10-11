using System.Text.Json;

namespace WordSearch.Helpers
{
    /// <summary>
    /// Helper qui définit les méthodes d'accès à l'API Datamuse.
    /// </summary>
    public static class DatamuseApiHelper
    {
        private const string API_URL                = "https://api.datamuse.com/";
        private const string ACTION_SPELLED_LIKE    = "words?sp=";

        /// <summary>
        /// Récupération d'une liste de mots correspondant à un pattern.
        /// </summary>
        /// <param name="searchPattern">Pattern de recherche.</param>
        /// <returns></returns>
        public static async Task<List<DatamuseApiResult>> GetRandomWord(string searchPattern)
        {
            List<DatamuseApiResult> words = null;

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                using (HttpClient client = new HttpClient() { BaseAddress = new Uri(DatamuseApiHelper.API_URL) })
                {
                    string path = $"{DatamuseApiHelper.ACTION_SPELLED_LIKE}{searchPattern}";

                    using (HttpResponseMessage response = await client.GetAsync(path))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string retJson = await response.Content.ReadAsStringAsync();
                            words = JsonSerializer.Deserialize<List<DatamuseApiResult>>(retJson);
                        }
                    }
                }
            }

            return words;
        }
    }
}

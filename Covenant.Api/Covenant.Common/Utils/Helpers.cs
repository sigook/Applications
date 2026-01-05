namespace Covenant.Common.Utils
{
    public static class Helpers
    {
        public static async Task<string> DownloadFileInBase64(string baseUrl, string filename)
        {
            if (string.IsNullOrEmpty(filename)) return string.Empty;
            try
            {
                using var client = new HttpClient();
                var responseMessage = await client.GetAsync($"{baseUrl}{filename}");
                if (!responseMessage.IsSuccessStatusCode) return string.Empty;
                string mediaType = responseMessage.Content.Headers.ContentType.MediaType;
                byte[] imageBytes = await responseMessage.Content.ReadAsByteArrayAsync();
                string base64String = Convert.ToBase64String(imageBytes);
                return $"data:{mediaType};base64,{base64String}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

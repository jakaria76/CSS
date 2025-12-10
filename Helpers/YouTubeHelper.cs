using System;
using System.Text.RegularExpressions;

namespace CSS.Helpers
{
    public static class YouTubeHelper
    {
        public static string ExtractYouTubeId(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            var regex = new Regex(@"(?:youtube(?:-nocookie)?\.com/(?:.*v=|v/|embed/|.*\/)|youtu\.be/)([A-Za-z0-9_-]{6,})",
                                  RegexOptions.IgnoreCase);
            var m = regex.Match(url);
            if (m.Success && m.Groups.Count > 1)
                return m.Groups[1].Value;

            return null;
        }

        public static bool IsYouTubeUrl(string url)
        {
            return ExtractYouTubeId(url) != null;
        }

        public static string BuildEmbedUrl(string youtubeId)
        {
            if (string.IsNullOrEmpty(youtubeId)) return null;
            return $"https://www.youtube.com/embed/{youtubeId}";
        }
    }
}

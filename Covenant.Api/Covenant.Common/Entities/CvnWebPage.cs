using Covenant.Common.Functionals;
using System.Text.RegularExpressions;

namespace Covenant.Common.Entities
{
    public class CvnWebPage
    {
        public const string WebpagePattern =
            @"^((http://)|(https://))?([a-zA-Z0-9]+[.])+[a-zA-Z]{2,4}(:\d+)?(/[~_.\-a-zA-Z0-9=&%@:]+)*\??[~_.\-a-zA-Z0-9=&%@:]*$";

        public string Url { get; }
        private CvnWebPage(string url) => Url = url;

        public static Result<CvnWebPage> Create(string url)
        {
            var error = $"'{url}' is not a valid url please use something like https://www.example.com";
            try
            {
                if (string.IsNullOrEmpty(url)) return Result.Fail<CvnWebPage>(ResultError.Create(nameof(url), error));
                Match match = Regex.Match(url, WebpagePattern);
                return match.Success && match.Value.Length == url.Length ? Result.Ok(new CvnWebPage(url)) : Result.Fail<CvnWebPage>(ResultError.Create(nameof(url), error));
            }
            catch (Exception)
            {
                return Result.Fail<CvnWebPage>(ResultError.Create(nameof(url), error));
            }
        }

        public static implicit operator string(CvnWebPage webPage) => webPage.Url;
    }
}
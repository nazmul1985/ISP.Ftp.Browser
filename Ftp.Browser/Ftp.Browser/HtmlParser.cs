using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace Ftp.Browser
{
    public class HtmlParser
    {
        private readonly Stack<string> _urlsToBrowse = new Stack<string>();
        private readonly List<string> _browsedLinks = new List<string>();
        private readonly string[] _videoExtensions = { ".AVI", ".M4A", ".M4V", ".MKV", ".MOV", ".MP4", ".MPA", ".MPE", ".MPEG", ".MPG" };
        private readonly string[] _subtitlesExtensions = { ".SRT", ".SUB", ".SBV" };
        private readonly string[] _excludeExtension = { ".TXT", ".TEXT", ".JPG" };

        private string _rootUrl;
        private string _currentRootUrl;

        public void ParseHtml(string url)
        {
            if (!this.IsValidUri(url))
            {
                this.ParseNextUrl();
                return;
            }
            if (string.IsNullOrWhiteSpace(this._rootUrl) && !this.IsUrlAccessable(url))
            {
                return;
            }
            if (this._browsedLinks.Contains(url))
            {
                this.ParseNextUrl();
                return;
            }
            this._browsedLinks.Add(url);
            this._currentRootUrl = url;
            if (string.IsNullOrWhiteSpace(this._rootUrl))
            {
                this._rootUrl = url;
            }
            var htmlWeb = new HtmlWeb();
            var htmlDocument = htmlWeb.Load(url);
            if (htmlDocument.DocumentNode.ChildNodes.Any() == false)
            {
                this.ParseNextUrl();
                return;
            }
            var links = htmlDocument.DocumentNode.SelectNodes("//a").ToList();
            foreach (var link in links)
            {
                var linkAttribute = link.Attributes["href"];
                var linkUrl = linkAttribute?.DeEntitizeValue;
                if (string.IsNullOrWhiteSpace(linkUrl))
                {
                    continue;
                }

                if (!this.IsValidUri(linkUrl))
                {
                    linkUrl = $"{this._currentRootUrl}/{linkUrl}";
                    if (!this.IsValidUri(linkUrl))
                    {
                        continue;
                    }
                }

                if (!this.IsUrlAccessable(linkUrl))
                {
                    continue;
                }

                Console.WriteLine($"{linkUrl}");

                if (this.IsVideo(linkUrl))
                {
                    this.ExtractVideoFileDetails(link);
                }
                else if (this.IsSubtitle(url))
                {
                    this.ExtractSubtitleDetails(link);
                }
                else
                {
                    this._urlsToBrowse.Push(linkUrl);
                }
            }

            this.ParseNextUrl();
            Console.ReadLine();
        }

        private void ParseNextUrl()
        {
            if (this._urlsToBrowse.Any())
            {
                this.ParseHtml(this._urlsToBrowse.Pop());
            }
        }

        private void ExtractSubtitleDetails(HtmlNode link)
        {

        }

        private bool IsUrlAccessable(string url)
        {
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFtp)
            {
                return this.IsFtpAccessable(url);
            }
            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                return this.IsHttpAccessable(url);
            }
            return false;
        }

        private bool IsHttpAccessable(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 3000;
            request.Method = "HEAD"; // As per Lasse's comment
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
        //request.Method = WebRequestMethods.Ftp.ListDirectory;
        //        request.Credentials = new NetworkCredential(user, password);
        //request.GetResponse();

        private bool IsFtpAccessable(string url)
        {
            var request = (FtpWebRequest)WebRequest.Create(url);
            request.Timeout = 3000;
            request.Method = WebRequestMethods.Ftp.ListDirectory; // As per Lasse's comment
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    var xx = response.GetResponseStream();
                    return response.StatusCode == FtpStatusCode.CommandOK;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void ExtractVideoFileDetails(HtmlNode url)
        {

        }

        private bool IsValidUri(string url)
        {
            Uri uri;
            var isValidUri = Uri.TryCreate(url, UriKind.Absolute, out uri);
            return isValidUri;
        }

        private bool IsVideo(string url)
        {
            //url = "ftp://172.16.0.2/Movies/Animation%20Movies/2016/Angry%20Birds%202016/hdtc-tangry.birds16.mkv";

            //var uri = new Uri(url);
            //if (uri.Scheme != Uri.UriSchemeFtp) return false;
            var extension = Path.GetExtension(url);
            return !string.IsNullOrWhiteSpace(extension) && this._videoExtensions.Contains(extension.ToUpperInvariant());
        }

        private bool IsSubtitle(string url)
        {
            var uri = new Uri(url);
            if (uri.Scheme != Uri.UriSchemeFtp) return false;
            var extension = Path.GetExtension(url);
            return !string.IsNullOrWhiteSpace(extension) && this._subtitlesExtensions.Contains(extension.ToUpperInvariant());
        }

        public IEnumerable<string> GetUrls()
        {
            return null;
        }

        public IEnumerable<string> GetVedioPaths()
        {
            return null;
        }
    }
}

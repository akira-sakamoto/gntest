/*
 * GNTEST - a Gracenote Web API test Program
 * Copyright: akira.sakamoto@gmail.com
 * License: MIT
 */

#define GNDEBUG
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


// XML definition for Gracenote Web API
namespace GracenoteXml.Model
{
    /// <summary>XML definition for Web API responses</summary>
    /// <remarks>all Gracenote responses are contained in a root RESPONSES element</remarks>
    /// <example>
    /// <code>
    ///   ＜RESPONSES＞
    ///     ＜RESPONSE＞
    ///          :
    ///     ＜/＜RESPONSE＞
    ///   ＜/RESPONSES＞
    /// </code>
    /// </example>
    [System.Xml.Serialization.XmlRoot("RESPONSES")]
    public class ResponsesModel
    {
        /// <summary>query responses</summary>
        [System.Xml.Serialization.XmlElement("RESPONSE")]
        public ResponseModel Response { get; set; }

        /// <summary>Error Message</summary>
        /// <remarks>this element shows error message when STATUS attribute is ERROR</remarks>
        [System.Xml.Serialization.XmlElement("MESSAGE")]
        public string Message { get; set; }
    }

    /// <summary>Web API responses</summary>
    public class ResponseModel
    {
        /// <summary>Status Reporting attribute: OK / MO_MATCH / ERROR</summary>
        /// <example>
        /// <code>
        ///   ＜RESPONSE STATUS="OK"＞
        /// </code>
        /// </example>
        [System.Xml.Serialization.XmlAttribute("STATUS")]
        public string Status { get; set; }

        /// <summary>Registration response</summary>
        [System.Xml.Serialization.XmlElement("USER")]
        public string User { get; set; }

        /// <summary>Paging result</summary>
        [System.Xml.Serialization.XmlElement("RANGE")]
        public RangeModel Range { get; set; }

        /// <summary>Album data</summary>
        [System.Xml.Serialization.XmlElement("ALBUM")]
        public List<GracenoteXml.Model.AlbumModel> Album { get; set; }
    }

    /// <summary>Album Data</summary>
    public class AlbumModel
    {
        /// <summary>Sequence number attribute</summary>
        [System.Xml.Serialization.XmlAttribute("ORD")]
        public string Ord { get; set; }

        /// <summary>Gracenote identifier</summary>
        [System.Xml.Serialization.XmlElement("GN_ID")]
        public string Gn_id { get; set; }

        /// <summary>Album artist's name</summary>
        [System.Xml.Serialization.XmlElement("ARTIST")]
        public string Artist { get; set; }

        /// <summary>Album's Title</summary>
        [System.Xml.Serialization.XmlElement("TITLE")]
        public string Title { get; set; }

        /// <summary>package language</summary>
        [System.Xml.Serialization.XmlElement("PKG_LANG")]
        public string Pkg_lang { get; set; }

        /// <summary>Release year of the album</summary>
        [System.Xml.Serialization.XmlElement("DATE")]
        public string Date { get; set; }

        /// <summary>Album's genre</summary>
        [System.Xml.Serialization.XmlElement("GENRE")]
        public List<GracenoteXml.Model.GenreModel> AGenre { get; set; }

        /// <summary>Number of the matched track for queries</summary>
        [System.Xml.Serialization.XmlElement("MATCHED_TRACK_NUM")]
        public string Matched_track_num { get; set; }

        /// <summary>Number of tracks in the album</summary>
        [System.Xml.Serialization.XmlElement("TRACK_COUNT")]
        public string Track_count { get; set; }

        /// <summary>Node for each track returned</summary>
        [System.Xml.Serialization.XmlElement("TRACK")]
        public List<GracenoteXml.Model.TrackModel> Track { get; set; }

        /// <summary>URL for the album cover art</summary>
        [System.Xml.Serialization.XmlElement("URL")]
        public string Url { get; set; }
    }

    /// <summary>Node for queries that return multiple albums</summary>
    public class RangeModel
    {
        /// <summary>Total number of matched albums</summary>
        [System.Xml.Serialization.XmlElement("COUNT")]
        public string Count { get; set; }

        /// <summary>Start position</summary>
        [System.Xml.Serialization.XmlElement("START")]
        public string Start { get; set; }

        /// <summary>End position</summary>
        [System.Xml.Serialization.XmlElement("END")]
        public string End { get; set; }
    }

    /// <summary>Track Data</summary>
    public class TrackModel
    {
        /// <summary>Track's number</summary>
        [System.Xml.Serialization.XmlElement("TRACK_NUM")]
        public string Track_num { get; set; }

        /// <summary>Gracenote identifier</summary>
        [System.Xml.Serialization.XmlElement("GN_ID")]
        public string Gn_id { get; set; }

        /// <summary>Track's artist</summary>
        [System.Xml.Serialization.XmlElement("ARTIST")]
        public string Artist { get; set; }

        /// <summary>Track's title</summary>
        [System.Xml.Serialization.XmlElement("TITLE")]
        public string Title { get; set; }

        /// <summary>Track's genre</summary>
        [System.Xml.Serialization.XmlElement("GENRE")]
        public List<GracenoteXml.Model.GenreModel> TGenre { get; set; }

        /// <summary>URL for the track's cover art</summary>
        [System.Xml.Serialization.XmlElement("URL")]
        public string Url { get; set; }
    }

    /// <summary>attribute for Genre</summary>
    public class GenreModel
    {
        /// <summary>Unique number of Genre</summary>
        [System.Xml.Serialization.XmlAttribute("NUM")]
        public string Num { get; set; }

        /// <summary>Unique number of Genre, too (I don't now the difference betweeen NUM and ID)</summary>
        [System.Xml.Serialization.XmlAttribute("ID")]
        public string Id { get; set; }

        /// <summary>Text description of genre</summary>
        [System.Xml.Serialization.XmlText()]
        public string Text { get; set; }
    }
}


// Gracenote Web API
namespace Gracenote
{
    /// <summary>
    /// local storage class
    /// </summary>
    public class Settings
    {
        private string _clientId;
        private string _clientTag;

        /// <summary>My client ID</summary>
        public string clientId { get {return _clientId;} set {_clientId = value;} }

        /// <summary>My client tag</summary>
        public string clientTag { get {return _clientTag;} set {_clientTag = value;} }

        /// <summary>Constructor</summary>
        public Settings()
        {
            _clientId = "";
            _clientTag = "";
        }
    }

    /// <summary>Reng parameter class</summary>
    public class OptRange 
    {
        /// <summary>Range Start</summary>
        public int Start { get; set; }
        /// <summary>Range End</summary>
        public int End { get; set; }

        /// <summary>Constructor</summary>
        public OptRange() { Start = End = 0; }
    }


    /// <summary>
    ///   Gracenote Web API wrapper
    /// </summary>
    public class WebApi
    {
        private static string _gnWebApi = "https://c{0}.web.cddbp.net/webapi/xml/1.0/";
        private Settings appSettings;
        private string _userId;
        private string _lang;

        /// <summary>
        ///   コンストラクタ
        ///   設定ファイル gnsetting.xml から clientId と clientTag を読み込む
        ///   アクセス用URL _gnWebApi を作る
        /// </summary>
        /// <example>
        ///   sample code for 'gnsetting.xml'
        /// <code>
        ///   ＜?xml version="1.0" encoding="utf-8"?＞
        ///   ＜Settings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"＞
        ///     ＜clientId＞XXXXXXXXXX＜/clientId＞
        ///     ＜clientTag＞YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY＜/clientTag＞
        ///   ＜/Settings＞
        /// </code>
        /// </example>
        public WebApi()
        {
            // Read configuration file
            const string configFile = "gnsetting.xml";
            System.IO.StreamReader sr = null;

            System.Xml.Serialization.XmlSerializer gnSettings = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            try {
                sr = new System.IO.StreamReader(configFile, new System.Text.UTF8Encoding(false));
                appSettings = (Settings)gnSettings.Deserialize(sr);
                _gnWebApi = _gnWebApi.Replace("{0}", appSettings.clientId);
            }
            catch (Exception) {
                Console.WriteLine("ERROR: could not read {0}", configFile);
                _gnWebApi = "";     // error result
                return;
            }
            finally {
                if (sr != null) {
                    sr.Close();
                }
                _userId = "";
                _lang = "";
            }
        }


        /// <summary>
        /// 信頼できないSSL証明書を「問題なし」にするメソッド
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/867sslavoidverify/sslavoidverify.html
        /// </summary>
        private static bool OnRemoteCertificateValidationCallback(
            Object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;  // 「SSL証明書の使用は問題なし」と示す
        }

        
        /// <summary>
        /// Post to the server
        /// </summary>
        /// <param name="postData">string</param>
        /// <returns>Response XML data</returns>
        public GracenoteXml.Model.ResponsesModel Post(string postData)
        {
#if (DEBUG)
            //this code is required when using untrusted SSL certification
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
#endif

            debugLog("postData: " + postData);

            WebRequest request = WebRequest.Create(_gnWebApi);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/xml";    //  "application/x-www-form-urlencoded"
                request.ContentLength = byteArray.Length;

            // send request
            Stream dataStream = request.GetRequestStream();
            try {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            catch (System.Net.WebException ex) {
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError) {
                    System.Net.HttpWebResponse errors = (System.Net.HttpWebResponse)ex.Response;
                    debugLog(errors.ResponseUri.ToString());
                    debugLog(errors.StatusCode + ":" + errors.StatusDescription);
                } else {
                    // TODO: エラー処理
                    debugLog(ex.Message);        
                }
            }
            finally {
                dataStream.Close();
            }

            // Get the response.
            WebResponse response = request.GetResponse();
                debugLog("Status = " + ((int)((HttpWebResponse)response).StatusCode).ToString() + ":" + ((HttpWebResponse)response).StatusDescription);
                // TODO: エラー処理

                dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                dataStream.Close();
            response.Close();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(responseFromServer));
            return Ms2Xml(ms);
        }

        /// <summary>get response status</summary>
        /// <param name="responses">responses which is received from API</param>
        /// <returns>response status</returns>
        public string getStatus(GracenoteXml.Model.ResponsesModel responses)
        {
            debugLog("STATUS:  " + responses.Response.Status);
            debugLog("MESSAGE: " + responses.Message);
            return responses.Response.Status;   // OK || NO_MATCH || ERROR
        }


        /// <summary>Register clientId and receive userId</summary>
        public void Register()
        {
            string postData = makeNode("QUERIES", makeNode("QUERY", "CMD=\"REGISTER\"", makeClientId()));
            GracenoteXml.Model.ResponsesModel responses = Post(postData);
            if (getStatus(responses) == "OK") {
                if (responses.Response.User != null) {
                    debugLog("user = " + responses.Response.User);
                    setUserId(responses.Response.User);
                }
            } else {
                debugLog("Registration Failed: " + responses.Response.Status);
            }
        }


         /// <summary>set userId</summary>
         /// <param name="userId">userId receieved from Web Api</param>
        public void setUserId(string userId)
        {
            _userId = userId;
        }

        /// <summary>set result language</summary>
        /// <param name="lang">language code</param>
        /// <remarks>three-character language code as defined by ISO 639-2</remarks>
        public void setLanguage(string lang)
        {
            _lang = lang;
            debugLog("setLanguage: " + _lang);
        }

        /// <summary>DEBUG: show registration info</summary>
        public void showRegistrationInfo()
        {
            Console.WriteLine("ApiUrl: {0}", _gnWebApi);
            Console.WriteLine("UserID: {0}", _userId);
            Console.WriteLine("Lang:   {0}", _lang);
        }

        
        /// <summary>
        /// Gracenote Web API: Track Search with artist name
        /// </summary>
        /// <param name="artistName">Artist name to query</param>
        /// <returns>XML result</returns>
        public GracenoteXml.Model.ResponsesModel trackSearch(string artistName)
        {
            return trackSearch(artistName, "", "", _lang);
        }

        /// <summary>
        /// Gracenote Web API: Track Search with artist name, album title, and track title
        /// </summary>
        /// <param name="artistName">Artist name to query</param>
        /// <param name="albumTitle">Album title to query</param>
        /// <param name="trackTitle">Track title to query</param>
        /// <returns>XML result</returns>
        public GracenoteXml.Model.ResponsesModel trackSearch(string artistName, string albumTitle, string trackTitle)
        {
            return trackSearch(artistName, albumTitle, trackTitle, _lang);
        }

        /// <summary>
        /// Gracenote Web API: Track Search with artist name, album title, track title, and language
        /// </summary>
        /// <param name="artistName">Artist name to query</param>
        /// <param name="albumTitle">Album title to query</param>
        /// <param name="trackTitle">Track title to query</param>
        /// <param name="language">Language, want to return</param>
        /// <returns>XML result</returns>
        public GracenoteXml.Model.ResponsesModel trackSearch(string artistName, string albumTitle, string trackTitle, string language)
        {
            return trackSearch(artistName, albumTitle, trackTitle, language, null);
        }

        /// <summary>
        /// Gracenote Web API: Track Search with artist name, album title, track title, and range
        /// </summary>
        /// <param name="artistName">Artist name to query</param>
        /// <param name="albumTitle">Album title to query</param>
        /// <param name="trackTitle">Track title to query</param>
        /// <param name="optRange">Range specified by Start and End</param>
        /// <returns>XML result</returns>
        public GracenoteXml.Model.ResponsesModel trackSearch(string artistName, string albumTitle, string trackTitle, OptRange optRange)
        {
            return trackSearch(artistName, albumTitle, trackTitle, "", optRange);
        }

        /// <summary>
        /// Gracenote Web API: Track Search with artist name, album title, track title, language, and range
        /// </summary>
        /// <param name="artistName">Artist name to query</param>
        /// <param name="albumTitle">Album title to query</param>
        /// <param name="trackTitle">Track title to query</param>
        /// <param name="language">Language, want to return</param>
        /// <param name="optRange">Range specified by Start and End</param>
        /// <returns>XML result</returns>
        public GracenoteXml.Model.ResponsesModel trackSearch(string artistName, string albumTitle, string trackTitle, string language, OptRange optRange)
        {
            if (_userId == "") {
                debugLog("Error: no registratered");
                return null;
            }
            if (artistName == "" && albumTitle == "" && trackTitle == "") {
                debugLog("parameter error: one of artistName, albumTitle or trackTitle must be set");
                return null;
            }
            // query body
            string body = "";
            if (artistName != "") { body += makeNode("TEXT", "TYPE=\"ARTIST\"", artistName); }
            if (albumTitle != "") { body += makeNode("TEXT", "TYPE=\"ALBUM_TITLE\"", albumTitle); }
            if (trackTitle != "") { body += makeNode("TEXT", "TYPE=\"TRACK_TITLE\"", trackTitle); }
            string query   = makeQuery("ALBUM_SEARCH", body, language, optRange);
            string queries = makeNode("QUERIES", makeAuth() + query);

            // post query
            GracenoteXml.Model.ResponsesModel responses = Post(queries);

            return responses;
        }

        private string makeAuth()
        {
            return makeNode("AUTH", makeClientId() + makeNode("USER", _userId));
        }

        /// <summary>make query string</summary>
        /// <param name="cmd">CMD parameter</param>
        /// <param name="body">query body</param>
        /// <returns>query string</returns>
        private string makeQuery(string cmd, string body)
        {
            return makeNode("QUERY", "CMD=\"" + cmd + "\"", body);
        }

        /// <summary>make query string</summary>
        /// <param name="cmd">CMD parameter</param>
        /// <param name="body">query body</param>
        /// <param name="language">result language</param>
        /// <param name="optRange">query range</param>
        /// <remarks>optRange can accept null, if not necessary</remarks>
        /// <returns>query string</returns>
        private string makeQuery(string cmd, string body, string language, OptRange optRange)
        {
            // make query string
            string lang = (language != "") ? makeNode("LANG", language) : "";
            // TODO: set option
            string range = "";
            if (optRange != null && (optRange.Start != 0 && optRange.End != 0)) {
                range = makeNode("RANGE", makeNode("START", optRange.Start) + makeNode("END", optRange.End));
            }
            
            return makeQuery(cmd, lang + range + body);
        }

        /**
        <summary>Album Fetch</summary>
        <param name="gnid">Gracenote Album ID to fetch</param>
        <example>
        <code>
        <![CDATA[
        <QUERIES>
          <AUTH>
            <CLIENT>client_id_string</CLIENT>
            <USER>user_id_string</USER>
          </AUTH>
          <LANG>eng</LANG>
          <COUNTRY>usa</COUNTRY>
          <QUERY CMD="ALBUM_FETCH">
            <GN_ID>a_gracenote_identifier</GN_ID>
          </QUERY>
        </QUERIES>
        ]]>
        </code>
        </example>
        */
        public GracenoteXml.Model.ResponsesModel albumFetch(string gnid)
        {
            return albumFetch(gnid, "", null);
        }

        /// <summary>Album Fetch with 3 parameters</summary>
        /// <param name="gnid">Gracenote Album ID to fetch</param>
        /// <param name="language">result language</param>
        /// <param name="optRange">query range</param>
        /// <returns>responses</returns>
        public GracenoteXml.Model.ResponsesModel albumFetch(string gnid, string language, OptRange optRange)
        {
            if (_userId == "") {
                debugLog("Error: no registratered");
                return null;
            }
            if (gnid == "") {
                debugLog("parameter error: gnid is not specified");
                return null;
            }
            // query body
            string body = makeNode("GN_ID", gnid);
            string query   = makeQuery("ALBUM_FETCH", body, language, optRange);
            string queries = makeNode("QUERIES", makeAuth() + query);

            // post query
            GracenoteXml.Model.ResponsesModel responses = Post(queries);
            return responses;
        }

        // make client id node
        private string makeClientId()
        {
            return makeNode("CLIENT", appSettings.clientId + "-" + appSettings.clientTag);
        }

        // make xml node <NODE>VALUE</NODE>
        private string makeNode(string node, string value)
        {
            return makeNode(node, "", value);
        }
        private string makeNode(string node, int value)
        {
            return makeNode(node, value.ToString());
        }

        // maake xml node <NODE OPT>VALUE</NODE>
        private string makeNode(string node, string opt, string value)
        {
            string pre = "<" + node + ((opt != "") ? " " + opt : "") + ">";
            return pre + value + "</" + node + ">";
        }


        /// <summary>convert MemoryStream to XML</summary>
        /// <param name="stream">MemoryStream</param>
        /// <returns>XML result</returns>
        public static GracenoteXml.Model.ResponsesModel Ms2Xml(MemoryStream stream)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GracenoteXml.Model.ResponsesModel));
            GracenoteXml.Model.ResponsesModel responses = (GracenoteXml.Model.ResponsesModel)serializer.Deserialize(stream);
            return responses;
        }

        /*
        TODO:
            getAlbumList    アルバムリストを返す
            getAlbum(index) 指定番号のアルバムを返す
            getArtist       アーティスト情報を返す
            getTitle(index) アルバム内のタイトルを返す
        */

        /// <summary>
        /// output debug message to stdout
        /// </summary>
        // [CallerFilePath] string path = ""
        public static void debugLog(
            string msg,
            [CallerMemberName] string name = "",
            [CallerLineNumber] int num = 0)
        {
#if (GNDEBUG)
            Console.Out.WriteLine("DEBUG: {0} {1} {2}", name, num, msg);
#endif
        }


        /// <summary>
        /// Get Album list in XML
        /// </summary>
        /// <param name="responses">Responses XML</param>
        /// <returns>Album list</returns>
        public static List<GracenoteXml.Model.AlbumModel> getAlbumList(GracenoteXml.Model.ResponsesModel responses)
        {
            return responses.Response.Album;
        }

        /// <summary>
        /// dump album list
        /// </summary>
        /// <param name="albumList">album list XML</param>
        public void ListAlbum(List<GracenoteXml.Model.AlbumModel> albumList)
        {
           // Album
            foreach (GracenoteXml.Model.AlbumModel album in albumList) {
                Console.WriteLine("ALBUM: ORD = {0}", album.Ord);
                Console.WriteLine("       GN_ID = {0}", album.Gn_id);
                Console.WriteLine("       ARTIST = {0}", album.Artist);
                Console.WriteLine("       TITLE = {0}", album.Title);
            
                Console.WriteLine("       PKG_LANG = {0}", album.Pkg_lang);
                Console.WriteLine("       DATE = {0}", album.Date);
                Console.WriteLine("       MATCHED_TRACK_NUM = {0}", album.Matched_track_num);
                Console.WriteLine("       TRACK_COUNT = {0}", album.Track_count);

                // album genre
                foreach (GracenoteXml.Model.GenreModel aGenre in album.AGenre) {
                    Console.WriteLine("       aGENRE: ID = {0}, Text = {1}", aGenre.Id, aGenre.Text);
                }

                foreach (GracenoteXml.Model.TrackModel track in album.Track) {
                    Console.WriteLine("       TRACK: TRACK_NUM = {0}", track.Track_num);
                    Console.WriteLine("              GN_ID = {0}", track.Gn_id);
                    Console.WriteLine("              ARTIST = {0}", track.Artist);
                    Console.WriteLine("              TITLE = {0}", track.Title);

                    foreach (GracenoteXml.Model.GenreModel tGenre in track.TGenre) {
                        Console.WriteLine("              tGENRE: ID = {0}, Text = {1}", tGenre.Id, tGenre.Text);
                    }

                    Console.WriteLine("              URL = {0}", track.Url);
                }

                Console.WriteLine("       URL1= {0}", album.Url);
                Console.WriteLine("--");
            }
        }


        /// <summary>
        /// show as csv format
        /// </summary>
        /// <param name="albumList">alubum list XML</param>
        public void CsvAlbum(List<GracenoteXml.Model.AlbumModel> albumList)
        {
            string aNum, aTitle, aArtist, aGnId;
            string tNum, tTitle, tArtist;

           // Album
            foreach (GracenoteXml.Model.AlbumModel album in albumList) {
                aNum = (album.Ord == null || album.Ord == "") ? "1" : album.Ord;
                aArtist = album.Artist;
                aTitle = album.Title;
                aGnId = album.Gn_id;

                foreach (GracenoteXml.Model.TrackModel track in album.Track) {
                    tNum = (track.Track_num == null || track.Track_num == "") ? "1" : track.Track_num;
                    tArtist = track.Artist;
                    tTitle = track.Title;
                    Console.WriteLine("{0}, \"{1}\", \"{2}\", \"{3}\", {4}, \"{5}\", \"{6}\"", aNum, aArtist, aTitle, aGnId, tNum, tArtist, tTitle);
                }
                Console.WriteLine();
            }
        }
    }
}


// main test program
namespace ConsoleApplication
{
    /// <summary>Test Program</summary>
    public class Program
    {
        // web api instance
        private static Gracenote.WebApi gn = null;

        private static int Str2Int(string str)
        {
            int n = 0;
            try {
                n = int.Parse(str);
            }
            catch (Exception) {
                n = 0;
            }
            return n;
        }
        /// <summary>Query test entry</summary>
        public static void queryTest()
        {
            string artistName = "";
            string albumTitle = "";
            string trackTitle = "";
            Gracenote.OptRange optRange = new Gracenote.OptRange();

            do {
                Console.Write("artistName: ");
                artistName = Console.ReadLine();

                Console.Write("albumTitle: ");
                albumTitle = Console.ReadLine();

                Console.Write("trackTitle: ");
                trackTitle = Console.ReadLine();

                Console.Write("startRange: ");
                optRange.Start = Str2Int(Console.ReadLine());

                Console.Write("endRange:   ");
                optRange.End = Str2Int(Console.ReadLine());

                Console.WriteLine();
            } while (artistName == "" && albumTitle == "" && trackTitle == "");

            GracenoteXml.Model.ResponsesModel res = gn.trackSearch(artistName, albumTitle, trackTitle, optRange);
            dumpResponses(res);
        }


        /// <summary>ALBUM_FETCH test</summary>
        public static void fetchTest()
        {
            string gnid = "";
            do {
                Console.Write("GN_ID: ");
                gnid = Console.ReadLine();
            } while (gnid == "");
            GracenoteXml.Model.ResponsesModel res = gn.albumFetch(gnid);
            dumpResponses(res);
        }


        /// <summary>dump responses</summary>
        public static void dumpResponses(GracenoteXml.Model.ResponsesModel res)
        {
            switch (gn.getStatus(res)) {
                case "OK":
                    Console.WriteLine("Status: OK");
                    if (res.Response.Range != null) {
                        GracenoteXml.Model.RangeModel range = (GracenoteXml.Model.RangeModel)res.Response.Range;
                        Console.WriteLine("RANGE: Count = {0}, Start = {1}, End = {2}", range.Count, range.Start, range.End);
                    }
                    // gn.ListAlbum(res.Response.Album);
                    gn.CsvAlbum(res.Response.Album);
                    break;

                case "NO_MATCH":
                    Console.WriteLine("Status: NO_MATCH");
                    break;

                case "ERROR":
                    Console.WriteLine("Status: ERROR");
                    break;
            }
        }


        /// <summary>Main entry</summary>
        public static void Main(string[] args)
        {
            int cmd;
            string str;

            gn = new Gracenote.WebApi();

            do {
                Console.WriteLine();
                gn.showRegistrationInfo();  // debug
                Console.WriteLine("cmd: 1: Regist, 2: Enter userId, 3: Enter Language, 4: Query, 5: Fetch, 0: Exit");
                Console.Write("> ");
                str = "";
                try {
                    cmd = int.Parse(Console.ReadLine());
                }
                catch (Exception) {
                    cmd = -1;
                }
                switch (cmd) {
                    case 1:
                        // Regist clientId and receive userId
                        gn.Register();
                        break;

                    case 2:
                        // set userID manually
                        do {
                            Console.Write("Enter userId: ");
                            str = Console.ReadLine();
                        } while (str == "");
                        gn.setUserId(str);
                        break;
                        
                    case 3:
                        // set result language
                        do {
                            Console.Write("Enter language (3 char code): ");
                            str = Console.ReadLine();
                        } while (str == "");
                        gn.setLanguage(str);
                        break;

                    case 4:
                        queryTest();
                        break;
                        
                    case 5:
                        fetchTest();
                        break;

                }
            } while (cmd != 0);
        }
    }
}

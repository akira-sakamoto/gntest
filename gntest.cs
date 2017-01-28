/**************************************************************
 * GNTEST - a Gracenote Web API test Program
 * Copyright: akira.sakamoto@gmail.com
 * License: MIT
 *************************************************************/

#define GNDEBUG
// https://msdn.microsoft.com/ja-jp/library/debx8sh9(v=vs.110).aspx
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
//using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GracenoteXml.Model
{
    [System.Xml.Serialization.XmlRoot("RESPONSES")]
    public class ResponsesModel
    {
        [System.Xml.Serialization.XmlElement("RESPONSE")]
        public ResponseModel Response { get; set; }

        [System.Xml.Serialization.XmlElement("MESSAGE")]
        public String Message { get; set; }
        

    }

    public class ResponseModel
    {
        [System.Xml.Serialization.XmlAttribute("STATUS")]
        public String Status { get; set; }      // OK | NO_MATCH | ERROR

        [System.Xml.Serialization.XmlElement("USER")]
        public string User { get; set; }

        [System.Xml.Serialization.XmlElement("RANGE")]
        public RangeModel Range { get; set; }

        [System.Xml.Serialization.XmlElement("ALBUM")]
        public System.Collections.Generic.List<GracenoteXml.Model.AlbumModel> Album { get; set; }
    }

    public class AlbumModel
    {
        [System.Xml.Serialization.XmlAttribute("ORD")]
        public String Ord { get; set; }

        [System.Xml.Serialization.XmlElement("GN_ID")]
        public String Gn_id { get; set; }

        [System.Xml.Serialization.XmlElement("ARTIST")]
        public String Artist { get; set; }

        [System.Xml.Serialization.XmlElement("TITLE")]
        public String Title { get; set; }

        [System.Xml.Serialization.XmlElement("PKG_LANG")]
        public String Pkg_lang { get; set; }

        [System.Xml.Serialization.XmlElement("DATE")]
        public String Date { get; set; }

        [System.Xml.Serialization.XmlElement("GENRE")]
        public System.Collections.Generic.List<GracenoteXml.Model.GenreModel> AGenre { get; set; }

        [System.Xml.Serialization.XmlElement("MATCHED_TRACK_NUM")]
        public String Matched_track_num { get; set; }

        [System.Xml.Serialization.XmlElement("TRACK_COUNT")]
        public String Track_count { get; set; }

        [System.Xml.Serialization.XmlElement("TRACK")]
        public System.Collections.Generic.List<GracenoteXml.Model.TrackModel> Track { get; set; }

        [System.Xml.Serialization.XmlElement("URL")]
        public String Url { get; set; }
    }

    public class RangeModel
    {
        [System.Xml.Serialization.XmlElement("COUNT")]
        public String Count { get; set; }

        [System.Xml.Serialization.XmlElement("START")]
        public String Start { get; set; }

        [System.Xml.Serialization.XmlElement("END")]
        public String End { get; set; }
    }

    public class TrackModel
    {
        [System.Xml.Serialization.XmlElement("TRACK_NUM")]
        public String Track_num { get; set; }

        [System.Xml.Serialization.XmlElement("GN_ID")]
        public String Gn_id { get; set; }

        [System.Xml.Serialization.XmlElement("ARTIST")]
        public String Artist { get; set; }

        [System.Xml.Serialization.XmlElement("TITLE")]
        public String Title { get; set; }

        [System.Xml.Serialization.XmlElement("GENRE")]
        public System.Collections.Generic.List<GracenoteXml.Model.GenreModel> TGenre { get; set; }

        [System.Xml.Serialization.XmlElement("URL")]
        public String Url { get; set; }
    }

    public class GenreModel
    {
        [System.Xml.Serialization.XmlAttribute("ID")]
        public String Id { get; set; }

        [System.Xml.Serialization.XmlText()]
        public String Text { get; set; }
    }
}


/**
 * Gracenote Web API
 */
namespace Gracenote
{
    // local storage
    public class Settings
    {
        private string _clientId;
        private string _clientTag;

        public string clientId { get {return _clientId;} set {_clientId = value;} }
        public string clientTag { get {return _clientTag;} set {_clientTag = value;} }
        public Settings()
        {
            _clientId = "";
            _clientTag = "";
        }
    }

    /**
     * Gracenote Web API wrapper
     */
    public class WebApi
    {
        private static string _gnWebApi = "https://c{0}.web.cddbp.net/webapi/xml/1.0/";
        //private static string _gnWebApi = "https://localhost/~asakamoto/posttest/receive.php";
        //private static string _gnWebApi = "http://localhost/~azira/posttest/receive.php";
        private Settings appSettings;
        private string _userId;
        private string _lang;

        /**
         * コンストラクタ
         * 設定ファイル gnsetting.xml から clientId と clientTag を読み込む
         * アクセス用URL _gnWebApi を作る
         *
         <?xml version="1.0" encoding="utf-8"?>
         <Settings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
           <clientId>XXXXXXXXXX</clientId>
           <clientTag>YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY</clientTag>
         </Settings>
         */
        public WebApi()
        {
            // Read configuration file
            const string configFile = "gnsetting.xml";
            System.IO.StreamReader sr = null;

            System.Xml.Serialization.XmlSerializer gnSettings = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            try {
                sr = new System.IO.StreamReader(configFile, new System.Text.UTF8Encoding(false));
                appSettings = (Settings)gnSettings.Deserialize(sr);
            }
            catch (Exception) {
                Console.WriteLine("ERROR: could not read {0}", configFile);
                return;
            }
            finally {
                if (sr != null) {
                    sr.Close();
                }
            }

            // make correct api address
            _gnWebApi = _gnWebApi.Replace("{0}", appSettings.clientId);
            debugLog("_gnWebApi = " +  _gnWebApi);

            _userId = "";
            _lang = "";
        }


        /**
         * 信頼できないSSL証明書を「問題なし」にするメソッド
         * http://www.atmarkit.co.jp/fdotnet/dotnettips/867sslavoidverify/sslavoidverify.html
         */
        private static bool OnRemoteCertificateValidationCallback(
            Object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            debugLog("OnRemoteCoertificateValidationCallback called");
            return true;  // 「SSL証明書の使用は問題なし」と示す
        }

        /**
         * Post
         * @param {string} postData
         */
        public string Post(string postData)
        {
            // https 対策
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);

            debugLog("postData: " + postData);

            WebRequest request = WebRequest.Create(_gnWebApi);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
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

            return responseFromServer;                        
        }


        /**
         * Gracenote Web API: Register
         */
        public void Register()
        {
            string postData = "<QUERIES><QUERY CMD=\"REGISTER\">" + makeClientId() + "</QUERY></QUERIES>";
            string res = Post(postData);
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(res));
            //GracenoteXml.Model.ResponsesModel xml = XMLtest(ms);
            // TODO: エラーチェック
            setUserId(XMLanalyze(ms));
        }

        /**
         * userId を設定する
         * @param {string} userId
         */
        public void setUserId(string userId)
        {
            _userId = userId;
        }

        /**
         * set result language (3 char code)
         * @param {string} lang
         */
        public void setLanguage(string lang)
        {
            _lang = lang;
            debugLog("setLanguage: " + _lang);
        }

        /**
         * DEBUG
         * show registration info
         */
        public void showRegistrationInfo()
        {
            Console.WriteLine("ApiUrl: {0}", _gnWebApi);
            Console.WriteLine("UserID: {0}", _userId);
            Console.WriteLine("Lang:   {0}", _lang);
        }


        /**
         * Gracenote Web API: trackSearch
         * @param {string} artistName
         * @param {string} albumTitle
         * @param {string} trackTitle
         * @param {string} lang
         */
        public void trackSearch(string artistName)
        {
            trackSearch(artistName, "", "", _lang);
        }
        public void trackSearch(string artistName, string albumTitle, string trackTitle)
        {
            trackSearch(artistName, albumTitle, trackTitle, _lang);
        }
        public void trackSearch(string artistName, string albumTitle, string trackTitle, string language)
        {
            if (_userId == "") {
                debugLog("Error: no registratered");
                return;
            }
            if (artistName == "" && albumTitle == "" && trackTitle == "") {
                debugLog("parameter error: one of artistName, albumTitle or trackTitle must be set");
                return;
            }
/*
            string lang = "eng";
            string artistName = "flying lotus";
            string albumTitle = "until the quiet comes";
            string trackTitle = "all";

            <QUERIES>
              <AUTH>
                <CLIENT>XXXXXXXXXX-YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY</CLIENT> 
                <USER>AAAAAAAAAAAAAAAAA-BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB</USER> 
              </AUTH> 
              <QUERY CMD="ALBUM_SEARCH"> 
                <TEXT TYPE="ARTIST">flying lotus</TEXT>
                <OPTION> 
                  <PARAMETER>SELECT_EXTENDED</PARAMETER> 
                  <VALUE>COVER,REVIEW,ARTIST_BIOGRAPHY,ARTIST_IMAGE,ARTIST_OET,MOOD,TEMPO</VALUE> 
                </OPTION>
                <OPTION> 
                  <PARAMETER>SELECT_DETAIL</PARAMETER> 
                  <VALUE>GENRE:3LEVEL,MOOD:2LEVEL,TEMPO:3LEVEL,ARTIST_ORIGIN:4LEVEL,ARTIST_ERA:2LEVEL,ARTIST_TYPE:2LEVEL</VALUE> 
                </OPTION>
                <OPTION> 
                  <PARAMETER>COVER_SIZE</PARAMETER> 
                  <VALUE>MEDIUM</VALUE> 
                </OPTION> 
              </QUERY> 
            </QUERIES>
*/
            // make query string
            string queries = "";
            string query = "";
            string lang = (language != "") ? makeNode("LANG", language) : "";
            string auth = makeNode("AUTH", makeClientId() + makeNode("USER", _userId));
            if (artistName != "") { query += makeNode("TEXT", "TYPE=\"ARTIST\"", artistName); }
            if (albumTitle != "") { query += makeNode("TEXT", "TYPE=\"ALBUM_TITLE\"", albumTitle); }
            if (trackTitle != "") { query += makeNode("TEXT", "TYPE=\"TRACK_TITLE\"", trackTitle); }
            // TODO: set option
            query   = makeNode("QUERY", "CMD=\"ALBUM_SEARCH\"", lang + query);
            queries = makeNode("QUERIES", auth + query);

            // post query
            string res = Post(queries);
//Console.WriteLine("res = {0}", res);
            // receive result
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(res));
            string s = XMLanalyze(ms);

Console.WriteLine("result = {0}", s);
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

        // maake xml node <NODE OPT>VALUE</NODE>
        private string makeNode(string node, string opt, string value)
        {
            string pre = "<" + node + ((opt != "") ? " " + opt : "") + ">";
            return pre + value + "</" + node + ">";
        }


        /**
         * XMLanalyze - analyze responses in memory stream
         * @param {stream} responses in memory stream
         */
        static string XMLanalyze(MemoryStream stream)
        {
debugLog("XMLanalyze:MemoryStream");            
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GracenoteXml.Model.ResponsesModel));
            GracenoteXml.Model.ResponsesModel responses = (GracenoteXml.Model.ResponsesModel)serializer.Deserialize(stream);
            return XMLanalyze(responses);
        }

        /**
         * XMLanalyze - analyze responses in XML
         * @param {responses} XML
         */
        static string XMLanalyze(GracenoteXml.Model.ResponsesModel responses)
        {
/*
            System.IO.FileStream fs = new System.IO.FileStream(source, System.IO.FileMode.Open);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GracenoteXml.Model.ResponsesModel));
            GracenoteXml.Model.ResponsesModel responses = (GracenoteXml.Model.ResponsesModel)serializer.Deserialize(fs);
*/
debugLog("XMLanalize #1");
            Console.WriteLine(String.Format("response = {0}, status = {1}", responses.Response, responses.Response.Status));
            if (responses.Response.Status == "ERROR") {
                Console.WriteLine(String.Format("message = {0}", responses.Message));
                return null;
            }
            // Register response
            if (responses.Response.User != null) {
                debugLog("Register Response: UserID = " + responses.Response.User);
                return responses.Response.User;
            }
            
            // Range exists
            if (responses.Response.Range != null) {
                GracenoteXml.Model.RangeModel range = (GracenoteXml.Model.RangeModel)responses.Response.Range;
                Console.WriteLine(String.Format("RANGE: Count = {0}, Start = {1}, End = {2}", range.Count, range.Start, range.End));
            }

            // Album
            foreach (GracenoteXml.Model.AlbumModel album in responses.Response.Album) {
                Console.WriteLine(String.Format("ALBUM: ORD = {0}", album.Ord));
                Console.WriteLine(String.Format("       GN_ID = {0}", album.Gn_id));
                Console.WriteLine(String.Format("       ARTIST = {0}", album.Artist));
                Console.WriteLine(String.Format("       TITLE = {0}", album.Title));
            
                Console.WriteLine(String.Format("       PKG_LANG = {0}", album.Pkg_lang));
                Console.WriteLine(String.Format("       DATE = {0}", album.Date));
                Console.WriteLine(String.Format("       MATCHED_TRACK_NUM = {0}", album.Matched_track_num));
                Console.WriteLine(String.Format("       TRACK_COUNT = {0}", album.Track_count));

                // album genre
                foreach (GracenoteXml.Model.GenreModel aGenre in album.AGenre) {
                    Console.WriteLine(String.Format("       aGENRE: ID = {0}, Text = {1}", aGenre.Id, aGenre.Text));
                }


                foreach (GracenoteXml.Model.TrackModel track in album.Track) {
                    Console.WriteLine(String.Format("       TRACK: TRACK_NUM = {0}", track.Track_num));
                    Console.WriteLine(String.Format("              GN_ID = {0}", track.Gn_id));
                    Console.WriteLine(String.Format("              ARTIST = {0}", track.Artist));
                    Console.WriteLine(String.Format("              TITLE = {0}", track.Title));

                    foreach (GracenoteXml.Model.GenreModel tGenre in track.TGenre) {
                        Console.WriteLine(String.Format("              tGENRE: ID = {0}, Text = {1}", tGenre.Id, tGenre.Text));
                    }

                    Console.WriteLine(String.Format("              URL = {0}", track.Url));
                }

                Console.WriteLine(String.Format("       URL1= {0}", album.Url));
                Console.WriteLine("--");
            }

//            fs.Close();

            return "dummy";
        }


        /*
        TODO:
            getAlbumList    アルバムリストを返す
            getAlbum(index) 指定番号のアルバムを返す
            getArtist       アーティスト情報を返す
            getTitle(index) アルバム内のタイトルを返す
        */

        // output debug message to stdout
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

    }
}



// main test program
namespace ConsoleApplication
{
    public class Program
    {
        private static Gracenote.WebApi gn = null;

        




        public static void queryTest()
        {
            string artistName = "";
            string albumTitle = "";
            string trackTitle = "";

            do {
                Console.Write("artistName: ");
                artistName = Console.ReadLine();

                Console.Write("albumTitle: ");
                albumTitle = Console.ReadLine();

                Console.Write("trackTitle: ");
                trackTitle = Console.ReadLine();

                Console.WriteLine();
            } while (artistName == "" && albumTitle == "" && trackTitle == "");

            gn.trackSearch(artistName, albumTitle, trackTitle);
        }


        public static void Main(string[] args)
        {
            int cmd;
            string str;

            gn = new Gracenote.WebApi();

            do {
                Console.WriteLine();
                gn.showRegistrationInfo();  // debug
                Console.WriteLine("cmd: 1: Regist, 2: Enter userId, 3: Enter Language, 4: Query, 0: Exit");
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
                }
            } while (cmd != 0);
        }
    }
}

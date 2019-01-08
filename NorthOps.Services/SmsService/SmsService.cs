using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;

namespace NorthOps.Services.SmsService
{
    public class Result
    {
        public Responses Response { get; set; }
        public string SessionId { get; set; }
        public string Code { get; set; }
    }

    public class Responses
    {
        public string Response { get; set; }
        public int Count { get; set; }
        public Messages Messages { get; set; }
        public string Token { get; set; }

    }

    public class Messages
    {
        public List<Message> Message { get; set; }
    }

    public class Message
    {
        public string Phone { get; set; }
        public string Content { get; set; }
    }
    public class Constants
    {
        public const string ERROR_LOGIN_USERNAME_WRONG = "108001";
        public const string ERROR_LOGIN_PASSWORD_WRONG = "108002";
        public const string ERROR_LOGIN_USERNAME_PWD_WRONG = "108006";
        public const string ERROR_WRONG_SESSION_TOKEN = "125003";
        public const string OK = "OK";
    }

    public class SmsService
    {
        public SmsService(string IpAddress, string UserName, string Password)
        {
            this.IpAddress = IpAddress;
            this.UserName = UserName;
            this.Password = Password;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }

        string VerificationToken(string content)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(content);
            var verToken = htmlDocument.DocumentNode.SelectNodes("//meta").First().Attributes["content"].Value;
            return verToken;
        }
        string GetSessionId(IRestResponse res)
        {
            return res.Cookies.FirstOrDefault(m => m.Name == "SessionID")?.Value;
        }

        public Result GetSmsList(string sessionId)
        {
            RestClient client = new RestClient($"http://{IpAddress}/html/smsinbox.html");

            var restRequest = new RestRequest();
            restRequest.AddCookie("SessionID", sessionId);
            var res = client.Execute(restRequest);


            client = new RestClient($"http://{IpAddress}");
            var token = VerificationToken(res.Content);
            restRequest = new RestRequest("api/sms/sms-list");
            restRequest.AddCookie("SessionID", sessionId);
            restRequest.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            restRequest.AddHeader("__RequestVerificationToken", token);
            restRequest.AddParameter("application/x-www-form-urlencoded", GetInbox(), ParameterType.RequestBody);
            res = client.Post(restRequest);


            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Content);
            var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
            var result = JsonConvert.DeserializeObject<Result>(fromXml);


            return result;
        }

        Result WebServerToken(string sessionId)
        {
            RestClient client = new RestClient($"http://{IpAddress}");
            var restRequest = new RestRequest("api/webserver/token");
            restRequest.AddCookie("SessionID", sessionId);
            var res = client.Execute(restRequest);


            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Content);
            var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
            var result = JsonConvert.DeserializeObject<Result>(fromXml);

            return result;
        }
        public Result Login()
        {
            RestClient client = new RestClient($"http://{IpAddress}/html/overview.html");
            var restRequest = new RestRequest();
            var res = client.Execute(restRequest);
            var sessionId = GetSessionId(res);

            var token =VerificationToken(res.Content);


            client = new RestClient($"http://{IpAddress}");
            restRequest = new RestRequest("api/user/login");

            restRequest.AddHeader("__RequestVerificationToken", token);
            restRequest.AddCookie("SessionID", sessionId);
            restRequest.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            var respassword = XmlPassword(token);
            restRequest.AddParameter("application/x-www-form-urlencoded", respassword, ParameterType.RequestBody);
            res = client.Post(restRequest);
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Content);
            var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
            var result = JsonConvert.DeserializeObject<Responses>(fromXml);
            return new Result() { Response = result, SessionId = GetSessionId(res) };
        }


        public Result sendSms(string sessionId)
        {
            RestClient client = new RestClient($"http://{IpAddress}/html/smsinbox.html");

            var restRequest = new RestRequest();
            restRequest.AddCookie("SessionID", sessionId);
            var res = client.Execute(restRequest);


            client = new RestClient($"http://{IpAddress}");
            var token = VerificationToken(res.Content);
            restRequest = new RestRequest("api/sms/send-sms");
            restRequest.AddCookie("SessionID", sessionId);
            restRequest.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            restRequest.AddHeader("__RequestVerificationToken", token);
            restRequest.AddParameter("application/x-www-form-urlencoded", SendData("222", "bal"), ParameterType.RequestBody);
            res = client.Post(restRequest);
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Content);
            var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
            var result = JsonConvert.DeserializeObject<Responses>(fromXml);
            return new Result() { Response = result };
        }


        public string Sha256(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        string SendData(string phoneNumber, string message)
        {
            //<?xml version: "1.0" encoding="UTF-8"?><request><Index>-1</Index><Phones><Phone>222</Phone></Phones><Sca></Sca><Content>bal</Content><Length>3</Length><Reserved>1</Reserved><Date>2018-11-29 16:08:30</Date></request>
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Index>-1</Index><Phones><Phone>{phoneNumber}</Phone></Phones><Sca></Sca><Content>{message}</Content><Length>{message.Length - 1}</Length><Reserved>1</Reserved><Date>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</Date></request>";

        }

        string GetInbox()
        {
            return "<?xml version: \"1.0\" encoding=\"UTF-8\"?><request><PageIndex>1</PageIndex><ReadCount>20</ReadCount><BoxType>1</BoxType><SortType>0</SortType><Ascending>0</Ascending><UnreadPreferred>0</UnreadPreferred></request>";
        }
        string XmlPassword(string verificationToken)
        {
            verificationToken = Base64Encode(Sha256(Password + Base64Encode(Sha256(Password)) + verificationToken));
            //psd = Base64Encode(psd);
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Username>{UserName}</Username><Password>{verificationToken}</Password><password_type>4</password_type></request>";
        }
    }
}

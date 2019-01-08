using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NorthOps.Services.SmsService;
using RestSharp;

namespace NorthOps.SendSMSServices
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SpeedWiFi speedWiFi = new SpeedWiFi(new Uri("http://192.168.254.254/"));
            var res = await speedWiFi.LoginAsync("user", "@l03e1t3");
          


        }

        string SessionId(IRestResponse res)
        {
            return res.Cookies.FirstOrDefault(m => m.Name == "SessionID")?.Value;
        }
        string VerificationToken(string content)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(content);
            var verToken = htmlDocument.DocumentNode.SelectNodes("//meta").First().Attributes["content"].Value;
            return verToken;
        }
        void sendSms(string sessionId)
        {
            RestClient client = new RestClient("http://192.168.8.1/html/smsinbox.html");

            var restRequest = new RestRequest();
            restRequest.AddCookie("SessionID", sessionId);
            var res = client.Execute(restRequest);


            client = new RestClient("http://192.168.8.1");
            var token = VerificationToken(res.Content);
            restRequest = new RestRequest("api/sms/send-sms");
            restRequest.AddCookie("SessionID", sessionId);
            restRequest.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            restRequest.AddHeader("__RequestVerificationToken", token);
            restRequest.AddParameter("application/x-www-form-urlencoded", SendData("222", "bal"), ParameterType.RequestBody);
            res = client.Post(restRequest);
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
        string XmlPassword(string psd)
        {
            psd = Base64Encode(Sha256("admin" + Base64Encode(Sha256("admin")) + psd));
            //psd = Base64Encode(psd);
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Username>admin</Username><Password>{psd}</Password><password_type>4</password_type></request>";
        }



    }

    public class Users
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int password_type { get; set; }
    }


}

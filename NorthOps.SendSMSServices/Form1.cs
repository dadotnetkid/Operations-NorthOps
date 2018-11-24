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
using RestSharp;

namespace NorthOps.SendSMSServices
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            RestClient restClient = new RestClient("http://192.168.8.1");

            RestRequest restRequest = new RestRequest();
            var res = restClient.Execute(restRequest);

            restRequest = new RestRequest("../api/user/state-login");

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(res.Content);
            var verToken = htmlDocument.DocumentNode.SelectNodes("//meta").First().Attributes["content"].Value;
           //  verToken = "Rh2vE59DV5IhCep7KwsXvnuUWILU187G";
            var SessionId = res.Cookies.FirstOrDefault(m => m.Name == "SessionID")?.Value;
            //restRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
          //  SessionId ="PnGYiUBTEVbt1/v5wNz6MjHrujBeYfJ91MUOiOeTm6HO0vxDxv53ElHAF3nnFwkDySr7PcOoJCNoxAx/lJHI6X6Zlt1lDZFX9muE/tk1T+MNxBMLMwzfQMqAuCAqtTOK";
            restRequest.AddCookie("SessionID",SessionId);
            var respassword = XmlPassword("Rh2vE59DV5IhCep7KwsXvnuUWILU187G");
            //restRequest.AddParameter("application/x-www-form-urlencoded", "<?xml version: \"1.0\" encoding=\"UTF-8\"?><request><Username>admin</Username><Password>MGM5MWM0ZmQ2NDFiZDAyMTc3ZDc3NDRkNWFhODhjNDg2MDA5OGQwODBlMmY0N2U0YzU5N2NjMDQ0YjkyNjFjMw==</Password><password_type>4</password_type></request>", ParameterType.RequestBody);
            restRequest.AddXmlBody(respassword);

             res = restClient.Execute(restRequest);



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
        string SendData()
        {
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Index>-1</Index><Phones><Phone>222</Phone></Phones><Sca></Sca><Content>bal1</Content><Length>4</Length><Reserved>1</Reserved><Date>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</Date></request>";

        }
        string XmlPassword(string psd)
        {
            psd = Base64Encode(Sha256("admin" + Base64Encode(Sha256("admin")) +psd));
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

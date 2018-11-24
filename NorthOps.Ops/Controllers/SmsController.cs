using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.ViewModels;
using RestSharp;

namespace NorthOps.Ops.Controllers
{
    public class SmsController : Controller
    {
        // GET: Sms
        public ActionResult Index()
        {
            RestClient restClient = new RestClient("http://192.168.8.1");
            RestRequest restRequest = new RestRequest();

            var res = restClient.Execute(restRequest);

            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(res.Content);

            var verToken = htmlDocument.DocumentNode.SelectNodes("//meta").First().Attributes["content"].Value;


            var SessionId = res.Cookies.FirstOrDefault(m => m.Name == "SessionID")?.Value;

            var model = new SmsViewModel()
            {
                VerificationToken=verToken,
                SessionId=SessionId,
                XmlPassword=XmlPassword(verToken)
            };


            return View(model);
        }

        public JsonResult Login(SmsViewModel model)
        {
            RestClient restClient = new RestClient("http://192.168.8.1");
            RestRequest restRequest = new RestRequest("../api/user/login");
            restRequest.AddCookie("SessionID", model.SessionId);
            restRequest.AddHeader("encrypt_transmit", "encrypt_transmit");
            restRequest.AddHeader("__RequestVerificationToken", model.VerificationToken);
            restRequest.AddXmlBody(model.Rsa);
            var res = restClient.Execute(restRequest);
            return Json("");
        }
        string XmlPassword(string token)
        {
            var psd = Base64Encode(Sha256("admin" + Base64Encode(Sha256("admin")) + token));
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Username>user</Username><Password>{psd}</Password><password_type>4</password_type></request>";
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
    }
}
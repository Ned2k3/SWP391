﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProjectSWP391.Models;
using ProjectSWP391.Models.Purchase;
using RestSharp;
using System.Net;
using System.Text;

namespace ProjectSWP391.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly SWP391_V4Context context;

        public PurchaseController(IMemoryCache cache, SWP391_V4Context context)
        {
            _cache = cache;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int productId)
        {
            /*using (WebClient client = new WebClient())
            {
                var htmlData = client.DownloadData("https://api.vietqr.io/v2/banks");
                var bankRawJson = Encoding.UTF8.GetString(htmlData);
                var listBankData = JsonConvert.DeserializeObject<Bank>(bankRawJson);
                ViewBag.Banks = listBankData.data;
            }*/
            var product = context.Products.Where(p => p.ProductId == productId).FirstOrDefault();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Index(decimal amount, string addInfo)
        {
            /*
                {
                  "accountNo": 113366668888,
                  "accountName": "QUY VAC XIN PHONG CHONG COVID",
                  "acqId": 970415,
                  "amount": 79000,
                  "addInfo": "Ung Ho Quy Vac Xin",
                  "format": "text",
                  "template": "compact"
                }
             */
/*            Console.WriteLine(amount);*/
            var apiRequest = new APIRequest()
            {
                acqId = 970422,
                accountNo = 616666123666,
                accountName = "PHAM THANH LONG",
                amount = (Convert.ToInt32(amount) * 23000),
                addInfo = addInfo,
                format = "text",
                template = "compact2"
            };

            /*            Console.WriteLine(apiRequest);*/
            var jsonRequest = JsonConvert.SerializeObject(apiRequest);

            //Use RestSharp to request
            var client = new RestClient("https://api.vietqr.io/v2/generate");
            var request = new RestRequest();

            request.Method = Method.Post;
            request.AddHeader("Accept", "application/json");

            request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);


            //Use RestSharp to response
            var response = client.Execute(request);
            var content = response.Content;
            var dataResult = JsonConvert.DeserializeObject<APIResponse>(content);

            string qrDataKey = SaveQRDataURLToCache(dataResult.data.qrDataURL);
            return RedirectToAction("QRPayment", new { qrDataKey });
        }
        private string SaveQRDataURLToCache(string qrDataURL)
        {
            string qrDataKey = Guid.NewGuid().ToString();
            _cache.Set(qrDataKey, qrDataURL, TimeSpan.FromMinutes(10));
            return qrDataKey;
        }

        public IActionResult QRPayment(string qrDataKey)
        {
            if (_cache.TryGetValue(qrDataKey, out string qrDataURL))
            {
                ViewBag.qrImagePath = qrDataURL;
                return View();
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

    }
}

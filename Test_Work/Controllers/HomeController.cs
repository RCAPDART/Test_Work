using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

using Test_Work.Helpers;
using Test_Work.Models;
using Test_Work.Repository;

namespace Test_Work.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlMinificationHelper _urlMinificationHelper;
        private readonly IRepository<Url> _urlRepository;

        public HomeController(IUrlMinificationHelper urlMinificationHelper, IRepository<Url> urlRepository)
        {
            _urlMinificationHelper = urlMinificationHelper;
            _urlRepository = urlRepository;

            //Create test data
            if (_urlRepository.RowCount() < 1)
            {
                var LongUrl = "https://www.yandex.ru";
                var hortUrl = _urlMinificationHelper.UrlMinificate(LongUrl);
                var url1 = new Url
                {
                    LongUrl = LongUrl,
                    ShortUrl = hortUrl,
                    CreatedOn = DateTime.Now,
                    RedirectCount = 0
                };

                LongUrl = "https://www.mail.ru";
                hortUrl = _urlMinificationHelper.UrlMinificate(LongUrl);

                var url2 = new Url
                {
                    LongUrl = LongUrl,
                    ShortUrl = hortUrl,
                    CreatedOn = DateTime.Now,
                    RedirectCount = 0
                };

                LongUrl = "https://www.google.ru";
                hortUrl = _urlMinificationHelper.UrlMinificate(LongUrl);

                var url3 = new Url
                {
                    LongUrl = LongUrl,
                    ShortUrl = hortUrl,
                    CreatedOn = DateTime.Now,
                    RedirectCount = 0
                };

                _urlRepository.Insert(url1);
                _urlRepository.Insert(url2);
                _urlRepository.Insert(url3);
            }
        }

        public IActionResult Index()
        {
            var res = _urlRepository.GetAll().ToList();

            return View(res);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string LongUrl)
        {
            var isUrlValid = UrlChecker(LongUrl);
            
            if (!isUrlValid)
            {
                ModelState.AddModelError("Url", $"URL: {LongUrl} is invalid");
            }

            if (ModelState.IsValid)
            {
                var _hortUrl = _urlMinificationHelper.UrlMinificate(LongUrl);
                var res = new Url()
                {
                    LongUrl = LongUrl,
                    ShortUrl = _hortUrl,
                    CreatedOn = DateTime.Now,
                    RedirectCount = 0
                };

                _urlRepository.Insert(res);
                return RedirectToAction("SuccessAction", res);
            };

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        public IActionResult Edit(int Id)
        {
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range");
            }

            var url = _urlRepository.GetById(Id);
            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                return View(url);
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int Id, string LongUrl)
        {
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range");
            }

            var isUrlValid = UrlChecker(LongUrl);

            if (!isUrlValid)
            {
                ModelState.AddModelError("UrlVale", $"URL: {LongUrl} is invalid");
            }

            var url = _urlRepository.GetById(Id);

            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                url.LongUrl = LongUrl;
                url.ShortUrl = _urlMinificationHelper.UrlMinificate(LongUrl);

                _urlRepository.Update(url);

                return RedirectToAction("SuccessAction", url);
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        public IActionResult Delete(int Id)
        {
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range"); ;
            }

            var url = _urlRepository.GetById(Id);

            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                return View(url);
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUrl(int Id)
        {
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range");
            }

            var url = _urlRepository.GetById(Id);

            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                _urlRepository.Delete(url);

                return RedirectToAction("Index");
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        public IActionResult SuccessAction(Url url)
        {
            return View(url);
        }

        [HttpGet]
        public IActionResult RedirectMin(string minUrl)
        {
            var url = _urlRepository.GetAll().Where(u => u.ShortUrl == minUrl).FirstOrDefault();

            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                url.RedirectCount++;
                _urlRepository.Update(url);

                return Redirect(url.LongUrl);
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        [HttpPost]
        public IActionResult IncreaseVisit([FromBody] int Id)
        {
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range");
            }

            var url = _urlRepository.GetById(Id);

            if (url == null)
            {
                ModelState.AddModelError("Url", "There is no URL in DB");
            }

            if (ModelState.IsValid)
            {
                url.RedirectCount++;

                _urlRepository.Update(url);

                return Ok();
            }

            var errors = ExtractErrors(ModelState);
            return View("Error", errors);

        }

        private bool UrlChecker(string url)
        {
            if (!(url.StartsWith("http://") || url.StartsWith("https://")))
            {
                return false;
            }

            Uri uriResult;
            bool tryCreateResult = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            if (tryCreateResult == true && uriResult != null)
                return true;
            else
                return false;
        }

        private List<string> ExtractErrors(ModelStateDictionary errors)
        {
            return ModelState.Select(x => x.Value.Errors)
                      .Where(y => y.Count > 0)
                      .SelectMany(x => x)
                      .Select(x => x.ErrorMessage)
                      .ToList();
        }

    }

}

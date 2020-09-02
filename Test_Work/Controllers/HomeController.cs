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
            // Why this logic is here? It should be called in startup if it is defined in the startup.cs
            //Create test data
            
            // Why not !_urlRepostiroy.Any()? Count is pretty expensive operation. 
            if (_urlRepository.RowCount() < 1)
            {
                var LongUrl = "https://www.yandex.ru";
                // I'm wondering why you are initializing this in this way. It is not the way of OOP (encapsulation). 
                // You are ruling public properties of the object from the outside. This should be done via methods, or via constructor.
                // CreatedOn should be always readonly and should be set up in the internal logic of the class.
                // Also 'CreatedOn' usually going with 'ModifiedOn' together. If it is applieble in your app.
                
                var hortUrl = _urlMinificationHelper.UrlMinificate(LongUrl); // Typo in "short". Resharper has plugin for checking typos.
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

                // Usually repositories are using inside of the services. Not inside controllers. 
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
        public IActionResult Create(string LongUrl) // why PascalCase?
        {
            var isUrlValid = UrlChecker(LongUrl);
            
            if (!isUrlValid)
            {
                ModelState.AddModelError("Url", $"URL: {LongUrl} is invalid");
            }

            // Try to use "Guardian" pattern. 
            // In the simple words - if you have check, which is blocking logic (!ModelState.IsValid) - check, that it is invalid - and return "bad response".
            // Don't go forward, if you met block. Also - it will decrease nesting.
            if (ModelState.IsValid) // What is difference between isUrlValid and ModelState.IsValid? It is confusing.
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
            // This is what I told about. In "if" statement - you return "SuccessAction" and in the end of the method - you returned "Error" action.
            var errors = ExtractErrors(ModelState);
            return View("Error", errors);
        }

        public IActionResult Edit(int Id)
        {
            // Also - no Guardian pattern. I'm watching in the end of the method and see "return Error" and I don't undestand what this method is doing.
            // I need to look at all method to undestand how it is working. You need to create logic in way "check - check - check - logic - return result", where every check is blocking check (if fails - return bad response).
            if (Id < 1)
            {
                ModelState.AddModelError("Id", $"Id: {Id} is out of range");
            }
            // Also - you don't need to go deeper, because if Id is bad - you don't need to try get it from DB - it is bad.
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
        public IActionResult Edit(int Id, string LongUrl) // Why PascalCase in arguments?
        {
            // Guardian. Again here and in the other methods below. Also logic of checkin Id is the same in all your methods and you copy-pasted it everywhere. 
            // It is duplicated logic, which must be moved to the separate method. 
            // Because if you will modify it in one place - you must modify it in all other places.
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

using System;
using System.Web.Mvc;
using PoliteCaptcha;

namespace Sample
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WithoutFallback()
        {
            return View();
        }

        [HttpPost, ValidateSpamPrevention]
        public ActionResult WithoutFallback(SendFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            // we ignore the actual request, because this is just a demo.
            return SendFeedback(request);
        }

        public ActionResult WithFallback()
        {
            return View();
        }

        [HttpPost, ValidateSpamPrevention]
        public ActionResult WithFallback(SendFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            // we ignore the actual request, because this is just a demo.
            return SendFeedback(request);
        }

        ActionResult SendFeedback(SendFeedbackRequest request)
        {
            return Content(@"Spam prevention valiation passed. If this was a real feedback form, we'd have sent your feedback. (<a href=""/"">Back to Home</a>)");
        }
    }
}

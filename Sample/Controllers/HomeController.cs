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

        public ActionResult WithBypass()
        {
            return View();
        }

        [HttpPost, ValidateSpamPrevention]
        public ActionResult WithBypass(SendFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            // we ignore the actual request, because this is just a demo.
            return SendFeedback(request);
        }

        public ActionResult WithAjaxFallback()
        {
            return View();
        }

        [HttpPost, ValidateSpamPrevention]
        public JsonResult WithAjaxFallback(SendFeedbackRequest request)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("PoliteCaptcha"))
                {
                    return Json(new ResultMessage { Success = false, ErrorSource = "PoliteCaptcha" });
                }
            }

            // we ignore the actual request, because this is just a demo.
            return Json(new ResultMessage { Success = true, Message = "Spam prevention validation passed. If this was a real feedback form, we'd have sent your feedback."});
        }

        ActionResult SendFeedback(SendFeedbackRequest request)
        {
            return Content(@"Spam prevention validation passed. If this was a real feedback form, we'd have sent your feedback. (<a href=""/"">Back to Home</a>)");
        }

        private class ResultMessage
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string ErrorSource { get; set; }
        }
    }
}

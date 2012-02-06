using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateSpamPreventionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");
            if (filterContext.HttpContext == null)
                throw new ArgumentException(ErrorMessage.HttpContextMustNotBeNull, "filterContext");
            if (filterContext.Controller == null 
                || filterContext.Controller.ViewData == null 
                || filterContext.Controller.ViewData.ModelState == null)
                throw new ArgumentException(ErrorMessage.ModelStateMustNotBeNull, "filterContext");

            ICaptchaValidator captchaValidator = null;
            if (DependencyResolver.Current != null)
                captchaValidator = DependencyResolver.Current.GetService<ICaptchaValidator>();
            if (captchaValidator == null)
                captchaValidator = new ReCaptchaValidator();

            Authorize(
                filterContext.HttpContext,
                filterContext.Controller.ViewData.ModelState,
                captchaValidator);
        }

        public void Authorize(
            HttpContextBase httpContext,
            ModelStateDictionary modelState,
            ICaptchaValidator captchaValidator)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            if (modelState == null)
                throw new ArgumentNullException("modelState");
            if (captchaValidator == null)
                throw new ArgumentNullException("captchaValidator");

            // First check to see if the polite "noCAPTCHA" spam prevention worked.
            var noCaptchaResponseIsValid = ValidateNoCaptchaResponse(httpContext);
            if (noCaptchaResponseIsValid)
                return;

            // If there wasn't a valid "noCAPTCHA" response, we then check for a CAPTCHA response (the fallback for spam prevention).
            var captchaResponseIsValid = captchaValidator.Validate(httpContext);
            if (captchaResponseIsValid)
                return;

            // This model state error serves two purposes: it means the action can check ModelState.IsValid as normal,
            // and it's also a signal to the HTML helper that the spam prevention check failed, and therefore to show a captcha.
            modelState.AddModelError(
                Const.ModelStateKey,
                string.Empty);
        }

        static bool ValidateNoCaptchaResponse(HttpContextBase httpContext)
        {
            var noCaptchaChallenge = httpContext.Request.Form[Const.NoCaptchaChallengeField];
            if (string.IsNullOrWhiteSpace(noCaptchaChallenge))
                return false;

            var noCaptchaResponse = httpContext.Request.Form[Const.NoCaptchaResponseField];
            if (string.IsNullOrWhiteSpace(noCaptchaResponse))
                return false;

            noCaptchaResponse = new string(noCaptchaResponse.Reverse().ToArray());

            return noCaptchaChallenge.Equals(noCaptchaResponse, StringComparison.OrdinalIgnoreCase);
        }
    }
}
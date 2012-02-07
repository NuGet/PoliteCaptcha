using System;
using System.Web;

namespace PoliteCaptcha
{
    public class BypassCaptchaValidator : ICaptchaValidator
    {
        /// <summary>
        /// This is a "fake" CAPTCHA validator that always returns true, effectively bypassing CAPTCHA validation.
        /// </summary>
        /// <param name="httpContext">The request's HTTP context.</param>
        /// <returns>The result of validation; always true.</returns>
        public bool Validate(HttpContextBase httpContext)
        {
            return true;
        }
    }
}

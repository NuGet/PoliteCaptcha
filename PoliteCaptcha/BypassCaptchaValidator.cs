using System;
using System.Web;

namespace PoliteCaptcha
{
    /// <summary>
    /// A "fake" CAPTCHA validator that effectively bypasses CAPTCHA validation.
    /// </summary>
    public class BypassCaptchaValidator : ICaptchaValidator
    {
        /// <summary>
        /// "Fakes" CAPTCHA validation (always returns true), effectively bypassing CAPTCHA validation.
        /// </summary>
        /// <param name="httpContext">The request's HTTP context.</param>
        /// <returns>The result of validation; always true.</returns>
        public bool Validate(HttpContextBase httpContext)
        {
            return true;
        }
    }
}

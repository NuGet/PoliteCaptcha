using System;
using System.Web;

namespace PoliteCaptcha
{
    /// <summary>
    /// Validates a CAPTCHA response.
    /// </summary>
    public interface ICaptchaValidator
    {
        /// <summary>
        /// Validates a CAPTCHA response.
        /// </summary>
        /// <param name="httpContext">The request's HTTP context.</param>
        /// <returns>The result of validation; true or false.</returns>
        bool Validate(HttpContextBase httpContext);
    }
}
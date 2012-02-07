using System;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    /// <summary>
    /// Generates CAPTCHA HTML.
    /// </summary>
    public interface ICaptchaGenerator
    {
        /// <summary>
        /// Generates CAPTCHA HTML.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The CAPTCHA's HTML.</returns>
        IHtmlString Generate(
            HtmlHelper htmlHelper,
            string fallbackMessage = null);
    }
}

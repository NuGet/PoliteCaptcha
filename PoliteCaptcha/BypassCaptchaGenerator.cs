using System;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    /// <summary>
    /// A "fake" CAPTCHA generator that effectively bypasses CAPTCHA generation.
    /// </summary>
    public class BypassCaptchaGenerator : ICaptchaGenerator
    {
        /// <summary>
        /// Generates "fake" CAPTCHA HTML (returns an empty string), effectively bypassing CAPTCHA generation.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The CAPTCHA's HTML.</returns>
        public IHtmlString Generate(
            HtmlHelper htmlHelper, 
            string fallbackMessage = null)
        {
            return new MvcHtmlString(string.Empty);
        }
    }
}

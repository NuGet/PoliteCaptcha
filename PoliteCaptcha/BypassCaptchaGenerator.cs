using System;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    public class BypassCaptchaGenerator : ICaptchaGenerator
    {
        /// <summary>
        /// This is a "fake" CAPTCHA generator that returns nothing, effectively bypassing CAPTCHA generation.
        /// </summary>
        /// <param name="httpHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The CAPTCHA's HTML.</returns>
        public IHtmlString Generate(
            HtmlHelper httpHelper, 
            string fallbackMessage = null)
        {
            return new MvcHtmlString(string.Empty);
        }
    }
}

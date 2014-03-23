using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    /// <summary>
    /// Handles the generation of the scripts necessary to use the captcha in an Ajax POST
    /// </summary>
    interface IAjaxCaptchaGenerator : ICaptchaGenerator
    {
        /// <summary>
        /// Generates the script (PoliteCaptchaCreate) allowing the creation of the captcha in javascript
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The script of the creation of the capctha</returns>
        IHtmlString GenerateCaptchaCreationScript(
            HtmlHelper htmlHelper, 
            string fallbackMessage = null);

        /// <summary>
        /// Generates the Html placeholder which be filled by the PoliteCaptcha.Create method
        /// </summary>
        /// <returns>The Hml placeholder</returns>
        IHtmlString GenerateHtmlPlaceHolder();
    }
}

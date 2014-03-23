using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PoliteCaptcha
{
    /// <summary>
    /// HTML Helpers for PoliteCaptcha' spam prevention.
    /// </summary>
    public static class SpamPreventionHtmlHelpers
    {
        /// <summary>
        /// Generates the form fields' HTML for spam prevention. Use inside of an ASP.NET MVC form.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The spam prevention form fields' HTML</returns>
        public static IHtmlString SpamPreventionFields(
            this HtmlHelper htmlHelper,
            string fallbackMessage = null)
        {
            ICaptchaGenerator captchaGenerator = null;
            if (DependencyResolver.Current != null)
                captchaGenerator = DependencyResolver.Current.GetService<ICaptchaGenerator>();
            if (captchaGenerator == null)
                captchaGenerator = new ReCaptchaGenerator();

            var useCaptcha = htmlHelper.ViewData.ModelState.ContainsKey(Const.ModelStateKey);
            if (useCaptcha)
                return captchaGenerator.Generate(htmlHelper, fallbackMessage);

            return htmlHelper.Hidden(Const.NoCaptchaChallengeField, Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// Generates the JavaScript required for spam prevention. Requires jQuery.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <returns>The spam prevention JavaScript.</returns>
        public static IHtmlString SpamPreventionScript(this HtmlHelper htmlHelper)
        {
            return new MvcHtmlString(
                string.Format(
@"<script>
    $(function () {{
        $('input[name=""{0}""]').each(function() {{
            var response = this.value.split('').reverse().join('');
            var form = this.form;
            $('<input>').attr({{
                type: 'hidden',
                name: '{1}',
                value: response
            }}).appendTo(form);
        }});
    }});
</script>", Const.NoCaptchaChallengeField, Const.NoCaptchaResponseField));
        }

        /// <summary>
        /// Generates the JavaScript required to show the captcha. Requires jQuery.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The spam prevention JavaScript.</returns>
        public static IHtmlString SpamPreventionAjaxCreationScript(
            this HtmlHelper htmlHelper,
            string fallbackMessage = null)
        {
            IAjaxCaptchaGenerator captchaGenerator = null;
            if (DependencyResolver.Current != null)
                captchaGenerator = DependencyResolver.Current.GetService<IAjaxCaptchaGenerator>();
            if (captchaGenerator == null)
                captchaGenerator = new ReCaptchaGenerator();

            return captchaGenerator.GenerateCaptchaCreationScript(htmlHelper, fallbackMessage);
        }

        /// <summary>
        /// Generates the Html placeholder required to show the captcha.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <returns>The spam prevention JavaScript.</returns>
        public static IHtmlString SpamPreventionAjaxPlaceHolder(this HtmlHelper htmlHelper)
        {
            IAjaxCaptchaGenerator captchaGenerator = null;
            if (DependencyResolver.Current != null)
                captchaGenerator = DependencyResolver.Current.GetService<IAjaxCaptchaGenerator>();
            if (captchaGenerator == null)
                captchaGenerator = new ReCaptchaGenerator();

            return new MvcHtmlString(htmlHelper.Hidden(Const.NoCaptchaChallengeField, Guid.NewGuid().ToString("N")) + captchaGenerator.GenerateHtmlPlaceHolder().ToHtmlString());
        }
    }
}
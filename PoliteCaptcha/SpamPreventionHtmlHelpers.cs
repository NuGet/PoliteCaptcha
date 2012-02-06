using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PoliteCaptcha
{
    public static class SpamPreventionHtmlHelpers
    {
        public static IHtmlString SpamPreventionFields(this HtmlHelper htmlHelper)
        {
            ICaptchaGenerator captchaGenerator = null;
            if (DependencyResolver.Current != null)
                captchaGenerator = DependencyResolver.Current.GetService<ICaptchaGenerator>();
            if (captchaGenerator == null)
                captchaGenerator = new ReCaptchaGenerator();

            var useCaptcha = htmlHelper.ViewData.ModelState.ContainsKey(Const.ModelStateKey);
            if (useCaptcha)
                return captchaGenerator.Generate(htmlHelper);

            return htmlHelper.Hidden(Const.NoCaptchaChallengeField, Guid.NewGuid().ToString("N"));
        }

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
    }
}
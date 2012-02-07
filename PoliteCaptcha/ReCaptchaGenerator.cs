using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;

namespace PoliteCaptcha
{
    public class ReCaptchaGenerator : ICaptchaGenerator
    {
        public IHtmlString Generate(
            HtmlHelper htmlHelper, 
            string fallbackMessage = null)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");

            var publicApiKey = ConfigurationManager.AppSettings[Const.ReCaptchaPublicKeyAppSettingKey];
            if (publicApiKey == null)
            {
                if (!htmlHelper.ViewContext.HttpContext.Request.IsLocal)
                    throw new InvalidOperationException(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest);

                publicApiKey = Const.ReCaptchaLocalhostPublicKey;
            }

            var privateApiKey = ConfigurationManager.AppSettings[Const.ReCaptchaPrivateKeyAppSettingKey];
            if (privateApiKey == null)
            {
                if (!htmlHelper.ViewContext.HttpContext.Request.IsLocal)
                    throw new InvalidOperationException(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest);

                privateApiKey = Const.ReCaptchaLocalhostPrivateKey;
            }

            var recaptchaControl = new RecaptchaControl
            {
                ID = Const.ReCaptchControlId,
                PublicKey = publicApiKey,
                PrivateKey = privateApiKey,
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());
            recaptchaControl.RenderControl(htmlWriter);

            var captchaHtml = htmlWriter.InnerWriter.ToString();
            var template = @"<div class=""editor-label""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"">{0}</span></span></div><div class=""editor-field"">{1}</div>";
            
            return new MvcHtmlString(string.Format(template, fallbackMessage ?? Const.DefaulFallbackMessage, captchaHtml));  
        }
    }
}

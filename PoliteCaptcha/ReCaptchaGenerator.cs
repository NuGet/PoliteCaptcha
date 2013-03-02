using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;

namespace PoliteCaptcha
{
    /// <summary>
    /// A CAPTCHA generator that uses reCAPTCHA; the default CAPTCHA generator used by PoliteCaptcha.
    /// </summary>
    public class ReCaptchaGenerator : ICaptchaGenerator
    {
        readonly IConfigurationSource _configSource;

        public ReCaptchaGenerator()
            : this(new DefaultConfigurationSource())
        {
        }

        public ReCaptchaGenerator(IConfigurationSource configSource)
        {
            _configSource = configSource;
        }

        /// <summary>
        /// Generates CAPTCHA HTML using reCAPTCHA.
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The reCAPTCHA HTML.</returns>
        public IHtmlString Generate(
            HtmlHelper htmlHelper,
            string fallbackMessage = null)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            IConfigurationSource configurationSource = GetConfigurationSource();
            var publicApiKey = configurationSource.GetConfigurationValue(Const.ReCaptchaPublicKeyAppSettingKey);
            if (publicApiKey == null)
            {
                if (!htmlHelper.ViewContext.HttpContext.Request.IsLocal)
                    throw new InvalidOperationException(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest);

                publicApiKey = Const.ReCaptchaLocalhostPublicKey;
            }

            var privateApiKey = configurationSource.GetConfigurationValue(Const.ReCaptchaPrivateKeyAppSettingKey);
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

            const string template = @"<div class=""PoliteCaptcha editor-field""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"">{0}</span></span>{1}</div>";
            return new MvcHtmlString(string.Format(template, fallbackMessage ?? Const.DefaulFallbackMessage, captchaHtml));  
        }

        private IConfigurationSource GetConfigurationSource()
        {
            if (_configSource != null)
            {
                return _configSource;
            }

            if (DependencyResolver.Current != null)
            {
                var resolvedConfigSource = DependencyResolver.Current.GetService<IConfigurationSource>();
                if (resolvedConfigSource != null)
                {
                    return resolvedConfigSource;
                }
            }

            return new DefaultConfigurationSource();
        }
    }
}

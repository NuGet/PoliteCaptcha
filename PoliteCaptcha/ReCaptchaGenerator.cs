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
    public class ReCaptchaGenerator : ICaptchaGenerator, IAjaxCaptchaGenerator
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

        /// <summary>
        /// Generates the script (PoliteCaptchaCreate) allowing the creation of the captcha in javascript using ReCaptcha
        /// </summary>
        /// <param name="htmlHelper">The view's HTML helper.</param>
        /// <param name="fallbackMessage">An optional message to display above the CAPTCHA when it is displayed as a fallback.</param>
        /// <returns>The script of the creation of the capctha</returns>
        public IHtmlString GenerateCaptchaCreationScript(
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

            string scheme = "http";
            if (htmlHelper.ViewContext.HttpContext.Request.IsSecureConnection)
            {
                scheme += "s";
            }

            return new MvcHtmlString(
                string.Format(
@"<script type=""text/javascript"" src=""{0}://www.google.com/recaptcha/api/js/recaptcha_ajax.js""></script>
<script>
    function PoliteCaptchaCreate() {{
        $('#PoliteCaptchaMessage').html(""{1}"");
                Recaptcha.create(""{2}"",
                                    ""ReCaptchaPlaceHolder"",
                                    {{
                                        theme: ""red"",
                                        callback: Recaptcha.focus_response_field
                                    }}
                                  );
    }}
</script>", scheme, fallbackMessage ?? Const.DefaulFallbackMessage, publicApiKey));
        }

        /// <summary>
        /// Generates the Html placeholder which be filled by the PoliteCaptcha.Create method using ReCaptcha
        /// </summary>
        /// <returns>The Hml placeholder</returns>
        public IHtmlString GenerateHtmlPlaceHolder()
        {
            return new MvcHtmlString(@"<div class=""PoliteCaptcha editor-field""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"" id=""PoliteCaptchaMessage""></span></span><div id=""ReCaptchaPlaceHolder""></div></div>");
        }
    }
}

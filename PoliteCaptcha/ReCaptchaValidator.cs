using System;
using System.Web;
using System.Web.Mvc;
using Recaptcha;

namespace PoliteCaptcha
{
    /// <summary>
    /// A CAPTCHA validator that uses reCAPTCHA.
    /// </summary>
    public class ReCaptchaValidator : ICaptchaValidator
    {
        readonly RecaptchaValidator recaptchaValidator;
        readonly IConfigurationSource configSource;

        public ReCaptchaValidator()
            : this(new DefaultConfigurationSource())
        {
        }

        public ReCaptchaValidator(IConfigurationSource configSource)
        {
            this.configSource = configSource;
            this.recaptchaValidator = new RecaptchaValidator();
        }
        
        /// <summary>
        /// Validates a CAPTCHA response using reCAPTCHA.
        /// </summary>
        /// <param name="httpContext">The request's HTTP context.</param>
        /// <returns>The result of validation; true or false.</returns>
        public bool Validate(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var configurationSource = DependencyResolver.Current.GetService<IConfigurationSource>();
            var privateApiKey = configurationSource.GetConfigurationValue(Const.ReCaptchaPrivateKeyAppSettingKey);
            
            if (privateApiKey == null)
            {
                if (!httpContext.Request.IsLocal)
                    throw new InvalidOperationException(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest);

                privateApiKey = Const.ReCaptchaLocalhostPrivateKey;
            }

            var challenge = httpContext.Request.Form[Const.ReCaptchaChallengeField];
            if (string.IsNullOrWhiteSpace(challenge))
                return false;

            var response = httpContext.Request.Form[Const.ReCaptchaResponseField];
            if (string.IsNullOrWhiteSpace(response))
                return false;

            recaptchaValidator.PrivateKey = privateApiKey;
            recaptchaValidator.RemoteIP = httpContext.Request.UserHostAddress;
            recaptchaValidator.Challenge = challenge;
            recaptchaValidator.Response = response;

            return recaptchaValidator.Validate().IsValid;
        }
    }
}
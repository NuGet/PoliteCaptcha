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
        readonly RecaptchaValidator _recaptchaValidator;
        readonly IConfigurationSource _configSource;

        public ReCaptchaValidator()
            : this(new DefaultConfigurationSource())
        {
        }

        public ReCaptchaValidator(IConfigurationSource configSource)
        {
            _configSource = configSource;
            _recaptchaValidator = new RecaptchaValidator();
        }
        
        /// <summary>
        /// Validates a CAPTCHA response using reCAPTCHA.
        /// </summary>
        /// <param name="httpContext">The request's HTTP context.</param>
        /// <returns>The result of validation; true or false.</returns>
        public bool Validate(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var configurationSource = GetConfigurationSource();
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

            _recaptchaValidator.PrivateKey = privateApiKey;
            _recaptchaValidator.RemoteIP = httpContext.Request.UserHostAddress;
            _recaptchaValidator.Challenge = challenge;
            _recaptchaValidator.Response = response;

            return _recaptchaValidator.Validate().IsValid;
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
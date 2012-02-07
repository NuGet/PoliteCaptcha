using System;
using System.Configuration;
using System.Web;
using Recaptcha;

namespace PoliteCaptcha
{
    public class ReCaptchaValidator : ICaptchaValidator
    {
        readonly RecaptchaValidator recaptchaValidator;

        public ReCaptchaValidator()
        {
            recaptchaValidator = new RecaptchaValidator();
        }
        
        public bool Validate(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var privateApiKey = ConfigurationManager.AppSettings[Const.ReCaptchaPrivateKeyAppSettingKey];
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
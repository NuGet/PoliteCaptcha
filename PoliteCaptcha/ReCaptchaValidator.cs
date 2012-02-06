using System;
using System.Web;
using Recaptcha;

namespace PoliteCaptcha
{
    public class ReCaptchaValidator : ICaptchaValidator
    {
        readonly RecaptchaValidator recaptchaValidator;

        public ReCaptchaValidator()
        {
            recaptchaValidator = new RecaptchaValidator
            {
                PrivateKey = Const.ReCaptchaLocalhostPrivateKey,
            };
        }
        
        public bool Validate(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var challenge = httpContext.Request.Form[Const.ReCaptchaChallengeField];
            if (string.IsNullOrWhiteSpace(challenge))
                return false;

            var response = httpContext.Request.Form[Const.NoCaptchaResponseField];
            if (string.IsNullOrWhiteSpace(response))
                return false;

            recaptchaValidator.Challenge = challenge;
            recaptchaValidator.Response = response;
            recaptchaValidator.RemoteIP = httpContext.Request.UserHostAddress;

            return recaptchaValidator.Validate().IsValid;
        }
    }
}
using System;
using System.Web;

namespace PoliteCaptcha
{
    public class ReCaptchaValidator : ICaptchaValidator
    {
        public bool Validate(HttpContextBase httpContext)
        {
            throw new Exception();
        }
    }
}
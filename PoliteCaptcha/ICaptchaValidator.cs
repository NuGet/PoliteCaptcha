using System;
using System.Web;

namespace PoliteCaptcha
{
    public interface ICaptchaValidator
    {
        bool Validate(HttpContextBase httpContext);
    }
}
using System;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    public interface ICaptchaGenerator
    {
        IHtmlString Generate(
            HtmlHelper httpHelper,
            string fallbackMessage = null);
    }
}

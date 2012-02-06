using System;
using System.Web;
using System.Web.Mvc;

namespace PoliteCaptcha
{
    public class ReCaptchaGenerator : ICaptchaGenerator
    {
        public IHtmlString Generate(HtmlHelper htmlHelper)
        {
            throw new Exception();
        }
    }
}

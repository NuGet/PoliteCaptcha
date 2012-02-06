using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;

namespace PoliteCaptcha
{
    public class ReCaptchaGenerator : ICaptchaGenerator
    {
        public IHtmlString Generate(HtmlHelper htmlHelper)
        {
            var recaptchaControl = new RecaptchaControl
            {
                ID = Const.ReCaptchControlId,
                PublicKey = Const.ReCaptchaLocalhostPublicKey,
                PrivateKey = Const.ReCaptchaLocalhostPrivateKey,
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());
            recaptchaControl.RenderControl(htmlWriter);
            return new MvcHtmlString(htmlWriter.InnerWriter.ToString());  
        }
    }
}

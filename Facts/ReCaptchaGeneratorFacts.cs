using Moq;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace PoliteCaptcha
{
    public class ReCaptchaGeneratorFacts
    {
        public class The_Method_Generate
        {
            [Fact]
            public void will_throw_if_HtmlHelper_is_null()
            {
                var generator = new ReCaptchaGenerator();

                Assert.Throws<ArgumentNullException>(() => generator.Generate(null));
            }

            [Fact]
            public void will_throw_if_no_public_api_key_and_request_is_not_local()
            {
                var stubHtmlHelper = CreateHtmlHelper(false);
                var generator = new ReCaptchaGenerator();

                var ex = Assert.Throws<InvalidOperationException>(() => generator.Generate(stubHtmlHelper));
                Assert.Equal(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest, ex.Message);
            }

            [Fact]
            public void will_throw_if_no_private_api_key_and_request_is_not_local()
            {
                var stubHtmlHelper = CreateHtmlHelper(false);
                var generator = new ReCaptchaGenerator();
                ConfigurationManager.AppSettings[Const.ReCaptchaPublicKeyAppSettingKey] = "dummyKey";

                try
                {
                    var ex = Assert.Throws<InvalidOperationException>(() => generator.Generate(stubHtmlHelper));
                    Assert.Equal(ErrorMessage.DefaultReCaptchApiKeysOnlyAllowedForLocalRequest, ex.Message);
                }
                finally
                {
                    ConfigurationManager.AppSettings[Const.ReCaptchaPublicKeyAppSettingKey] = null;
                }
            }

            [Fact]
            public void will_generate_expected_html_with_no_secure_connection()
            {
                SetHttpContextCurrent();
                var stubHtmlHelper = CreateHtmlHelper();
                var generator = new ReCaptchaGenerator();

                var html = generator.Generate(stubHtmlHelper);
                Assert.Equal(@"<div class=""PoliteCaptcha editor-field""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"">Your request failed spam prevention. You must complete the CAPTCHA form below to proceed.</span></span><script type=""text/javascript"">
		var RecaptchaOptions = {
			theme : '',
			tabindex : 0
		};

</script><script type=""text/javascript"" src=""http://www.google.com/recaptcha/api/challenge?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"">

</script><noscript>
		<iframe src=""http://www.google.com/recaptcha/api/noscript?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"" width=""500"" height=""300"" frameborder=""0"">

		</iframe><br /><textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea><input name=""recaptcha_response_field"" value=""manual_challenge"" type=""hidden"" />
</noscript></div>",
            html.ToHtmlString());
            }

            [Fact]
            public void will_generate_expected_html_with_secure_connection()
            {
                SetHttpContextCurrent();
                var stubHtmlHelper = CreateHtmlHelper(isRequestSecureConnection: true);
                var generator = new ReCaptchaGenerator();

                var html = generator.Generate(stubHtmlHelper);
                Assert.Equal(@"<div class=""PoliteCaptcha editor-field""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"">Your request failed spam prevention. You must complete the CAPTCHA form below to proceed.</span></span><script type=""text/javascript"">
		var RecaptchaOptions = {
			theme : '',
			tabindex : 0
		};

</script><script type=""text/javascript"" src=""https://www.google.com/recaptcha/api/challenge?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"">

</script><noscript>
		<iframe src=""https://www.google.com/recaptcha/api/noscript?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"" width=""500"" height=""300"" frameborder=""0"">

		</iframe><br /><textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea><input name=""recaptcha_response_field"" value=""manual_challenge"" type=""hidden"" />
</noscript></div>",
            html.ToHtmlString());
            }

            [Fact]
            public void will_generate_expected_html_with_all_secure_connection()
            {
                SetHttpContextCurrent(isHttpContextCurrentRequestSecureConnection: true);
                var stubHtmlHelper = CreateHtmlHelper(isRequestSecureConnection: true);
                var generator = new ReCaptchaGenerator();

                var html = generator.Generate(stubHtmlHelper);
                Assert.Equal(@"<div class=""PoliteCaptcha editor-field""><span class=""field-validation-error"" data-valmsg-for=""PoliteCaptcha""><span htmlfor=""PoliteCaptcha"">Your request failed spam prevention. You must complete the CAPTCHA form below to proceed.</span></span><script type=""text/javascript"">
		var RecaptchaOptions = {
			theme : '',
			tabindex : 0
		};

</script><script type=""text/javascript"" src=""https://www.google.com/recaptcha/api/challenge?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"">

</script><noscript>
		<iframe src=""https://www.google.com/recaptcha/api/noscript?k=6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw"" width=""500"" height=""300"" frameborder=""0"">

		</iframe><br /><textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea><input name=""recaptcha_response_field"" value=""manual_challenge"" type=""hidden"" />
</noscript></div>",
            html.ToHtmlString());
            }
        }

        // SEcureConnection
        // fallback message

        static HtmlHelper CreateHtmlHelper(
            bool isRequestLocal = true,
            bool isRequestSecureConnection = false)
        {
            var stubHttpContext = new Mock<HttpContextBase>();
            var stubRequest = new Mock<HttpRequestBase>();
            var stubControllerBase = new Mock<ControllerBase>();
            var stubView = new Mock<IView>();

            var stubViewContext = new Mock<ViewContext>(
              new ControllerContext(
                stubHttpContext.Object,
                new RouteData(),
                stubControllerBase.Object),
              stubView.Object,
              new ViewDataDictionary(),
              new TempDataDictionary(),
              new StreamWriter(new MemoryStream()));

            var stubViewDataContainer = new Mock<IViewDataContainer>();

            stubViewContext.Setup(stub => stub.HttpContext).Returns(stubHttpContext.Object);
            stubHttpContext.Setup(stub => stub.Request).Returns(stubRequest.Object);
            stubRequest.Setup(stub => stub.IsLocal).Returns(isRequestLocal);
            stubRequest.Setup(stub => stub.IsSecureConnection).Returns(isRequestSecureConnection);

            return new HtmlHelper(stubViewContext.Object, stubViewDataContainer.Object);
        }

        static void SetHttpContextCurrent(bool isHttpContextCurrentRequestSecureConnection = false)
        {
            var stubHttpWorkerRequest = new Mock<HttpWorkerRequest>();
            stubHttpWorkerRequest.Setup(stub => stub.IsSecure()).Returns(isHttpContextCurrentRequestSecureConnection);
            stubHttpWorkerRequest.Setup(stub => stub.GetRawUrl()).Returns("/foo/page.aspx/tail?param=bar");
            HttpContext.Current = new HttpContext(stubHttpWorkerRequest.Object);
        }
    }
}

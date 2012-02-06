using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Moq;
using Xunit;

namespace PoliteCaptcha
{
    public class ValidateSpamPreventionAttributeFacts
    {
        public class The_Authorize_method
        {
            [Fact]
            public void will_throw_if_HTTP_context_is_null()
            {
                var attribute = new ValidateSpamPreventionAttribute();

                Assert.Throws<ArgumentNullException>(() => 
                    attribute.Authorize(
                        null,
                        new ModelStateDictionary(), 
                        new Mock<ICaptchaValidator>().Object));
            }

            [Fact]
            public void will_throw_if_model_state_is_null()
            {
                var attribute = new ValidateSpamPreventionAttribute();

                Assert.Throws<ArgumentNullException>(() =>
                    attribute.Authorize(
                        new Mock<HttpContextBase>().Object,
                        null,
                        new Mock<ICaptchaValidator>().Object));
            }

            [Fact]
            public void will_throw_if_CAPTCHA_validator_is_null()
            {
                var attribute = new ValidateSpamPreventionAttribute();

                Assert.Throws<ArgumentNullException>(() =>
                    attribute.Authorize(
                        new Mock<HttpContextBase>().Object,
                        new ModelStateDictionary(), 
                        null));
            }

            [Fact]
            public void will_not_add_a_model_state_error_when_the_nocaptcha_response_is_valid()
            {
                var stubHttpContext = CreateHttpContext(
                    noCaptchaChallenge: "1234",
                    noCaptchaResponse: "4321");
                var modelState = new ModelStateDictionary();
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    new Mock<ICaptchaValidator>().Object);

                Assert.False(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_not_add_a_model_state_error_when_the_nocaptcha_response_is_missing_but_the_captcha_response_is_valid()
            {
                var stubHttpContext = CreateHttpContext();
                var modelState = new ModelStateDictionary();
                var stubCaptchaValiator = new Mock<ICaptchaValidator>();
                stubCaptchaValiator.Setup(stub => stub.Validate(It.IsAny<HttpContextBase>())).Returns(true);
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    stubCaptchaValiator.Object);

                Assert.False(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_not_add_a_model_state_error_when_the_nocaptcha_response_is_invalid_but_the_captcha_response_is_valid()
            {
                var stubHttpContext = CreateHttpContext();
                var modelState = new ModelStateDictionary();
                var stubCaptchaValiator = new Mock<ICaptchaValidator>();
                stubCaptchaValiator.Setup(stub => stub.Validate(It.IsAny<HttpContextBase>())).Returns(true);
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    stubCaptchaValiator.Object);

                Assert.False(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_add_a_model_state_error_when_both_the_nocaptcha_and_captcha_response_is_invalid()
            {
                var stubHttpContext = CreateHttpContext();
                var modelState = new ModelStateDictionary();
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    new Mock<ICaptchaValidator>().Object);

                Assert.True(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_consider_nocaptcha_invalid_when_challenge_is_missing()
            {
                var stubHttpContext = CreateHttpContext(noCaptchaResponse: "1234");
                var modelState = new ModelStateDictionary();
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    new Mock<ICaptchaValidator>().Object);

                Assert.True(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_consider_nocaptcha_invalid_when_response_is_missing()
            {
                var stubHttpContext = CreateHttpContext(noCaptchaChallenge: "1234");
                var modelState = new ModelStateDictionary();
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    new Mock<ICaptchaValidator>().Object);

                Assert.True(modelState.ContainsKey(Const.ModelStateKey));
            }

            [Fact]
            public void will_consider_nocaptcha_invalid_when_response_is_not_the_reverse_of_the_challenge()
            {
                var stubHttpContext = CreateHttpContext(
                    noCaptchaChallenge: "1234",
                    noCaptchaResponse: "1234");
                var modelState = new ModelStateDictionary();
                var attribute = new ValidateSpamPreventionAttribute();

                attribute.Authorize(
                    stubHttpContext.Object,
                    modelState,
                    new Mock<ICaptchaValidator>().Object);

                Assert.True(modelState.ContainsKey(Const.ModelStateKey));
            }
        }

        static Mock<HttpContextBase> CreateHttpContext(
            string noCaptchaChallenge = null,
            string noCaptchaResponse = null)
        {
            var stubHttpContext = new Mock<HttpContextBase>();
            var stubHttpRequest = new Mock<HttpRequestBase>();
            var stubForm = new NameValueCollection();
            stubForm[Const.NoCaptchaChallengeField] = noCaptchaChallenge;
            stubForm[Const.NoCaptchaResponseField] = noCaptchaResponse;

            stubHttpContext.Setup(stub => stub.Request).Returns(stubHttpRequest.Object);
            stubHttpRequest.Setup(stub => stub.Form).Returns(stubForm);

            return stubHttpContext;
        }
    }
}

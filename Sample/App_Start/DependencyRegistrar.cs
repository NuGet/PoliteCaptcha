using System;
using System.Web;
using AnglicanGeek.SimpleContainer;
using PoliteCaptcha;

namespace Sample.App_Start
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDependencies(IDependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterCreator<ICaptchaGenerator>(() =>
            {
                if (HttpContext.Current.Request.RawUrl.Contains("WithBypass"))
                    return new BypassCaptchaGenerator();

                return new ReCaptchaGenerator();
            });

            dependencyRegistry.RegisterCreator<ICaptchaValidator>(() =>
            {
                if (HttpContext.Current.Request.RawUrl.Contains("WithBypass"))
                    return new BypassCaptchaValidator();

                return new ReCaptchaValidator();
            });
        }
    }
}
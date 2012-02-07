PoliteCaptcha is a spam prevention library for use with ASP.NET MVC 3 forms. 

Forcing humans to fill in forms with hard-to-read images because we can't tell them apart from maliciously-crafted computer programs is rude. PoliteCaptcha attempts to verify that the user's agent is a real web browser (via JavaScript and DOM manipulation, [using a technique adapted from Sam Saffron](http://samsaffron.com/archive/2011/10/04/Spam+bacon+sausage+and+blog+spam+a+JavaScript+approach)) before falling back to the use of a rude CAPTCHA (by default, reCAPTCHA--if you're going to be rude, at least do some good while you're at it). Very few spam programs run within a full web browser or have full support for JavaScript and the DOM, so this thwarts nearly all automated spam programs from exploiting your ASP.NET MVC app's forms.

## Installing PoliteCaptcha	

Install PoliteCaptcha via NuGet: `Install-Package PoliteCaptcha`. If you cannot use NuGet for some reason, you can build the source yourself by cloning this repository, then running the Build-Solution.ps1 script in the repo's root, and then getting the PoliteCaptcha.dll from the _build folder.

## Configuring PoliteCaptcha

reCAPTCHA requires a public and private API key. You must specify these keys in your app's configuration, as follows:

``` xml
<appSettings>
  <add key="reCAPTCHA::PublicKey" value="6LehOM0SAAAAAPgsjOy-6_grqy1JiB_W_jJa_aCw" />
  <add key="reCAPTCHA::PrivateKey" value="6LehOM0SAAAAAC5LsEpHoyyMqJcz7f_zEfqm66um" />
</appSettings>
```

PoliteCaptcha uses default API keys for reCAPTCHA that only work for requests to http://localhost. If the API key app settings aren't configured when generating or validating reCAPTCHA for non-local requests, an invalid operation exception will be thrown.

## Using PoliteCaptcha

Using PoliteCaptcha is very similar to using ASP.NET MVC's built-in anti-forgery support. First, invoke the `Html.SpamPreventionFields()` and `Html.SpamPreventionScript()` HTML helpers in your views that have forms needing spam prevention, to render the required form fields and JavaScript. Next, add the `[ValidateSpamPrevention]` attribute to the controller actions that process those forms. For example:

``` csharp
// in your controller, add the ValidateSpamPrevention attribute to actions that handle forms
[HttpPost, ValidateSpamPrevention]
public ActionResult RegisterMember(RegisterMemberRequest request)
{
	// ...
}
```

```
@* in your view's form, invoke the SpamPreventionFields() HTML helper *@
@using (Html.BeginForm())
{
    @Html.EditorForModel()
    @Html.SpamPreventionFields()
    <input type="submit" value="Submit" />
}

@* in your view's scripts section (or in the layout), invoke the SpamPreventionScript() HTML helper *@
@section PostFooter {
	<script src="@Url.Content("~/Scripts/jquery-1.6.2.min.js")"></script>
    <script src="@Url.Content("~/Scripts/jquery.validate.min.js")" type="text/javascript"></script>
    <script src="@Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js")" type="text/javascript"></script>
    @Html.SpamPreventionScript()
}
```

### Falling Back to CAPTCHA

You don't have to add any code to trigger the fallback to a CAPTCHA when the polite spam prevention fails; it's all handled through ASP.NET MVC model state. If your controller action follows the typical patterns for using model state, the spam prevention will just work.

## Q & A
**How do I bypass the CAPTCHA during development or test automation?**
PoliteCaptcha includes a BypassCaptchaGenerator and BypassCaptchaValidator which bypass all CAPTCHA generation and validation. You can selectively register these implementations of the [`ICaptchaGenerator`](https://github.com/NuGet/PoliteCaptcha/blob/master/PoliteCaptcha/ICaptchaGenerator.cs) and [`ICaptchaValidator`](https://github.com/NuGet/PoliteCaptcha/blob/master/PoliteCaptcha/ICaptchaValidator.cs) interfaces in the current dependency resolver as needed; see [Bypassing the Fallback CAPTCHA](https://github.com/NuGet/PoliteCaptcha/wiki/Bypassing-the-Fallback-CAPTCHA) for more information.

**What happens if JavaScript is disabled?**
The polite spam prevention requires JavaScript; if it is disabled, spam prevention falls back to a rude CAPTCHA so that the user can still use the form.

**I don't use jQuery; can I still use PoliteCaptcha?**
We plan to remove the dependency on jQuery eventually, but for now, it is required.

**I use a different CAPTCHA (i.e., not reCAPTCHA); can I make it more polite?**
PoliteCaptcha uses two interfaces for CAPTCHA: one to get the CAPTCHA's form fields (and associated HTML), and one to validate the user agent's response. Concrete implementations for these interfaces are located through ASP.NET MVC's dependency resolver (and if one doesn't exist, a default reCAPTCHA implementation is used). So, if you can make your CAPTCHA work through these two interfaces, you can use it with PoliteCaptcha. (If you can't make your captcha work with these interfaces, please let us know.)

**Can I change the error message that is displayed when PoliteCaptcha falls back to CAPTCHA?**
Yes, the `Html.SpamPreventionFields()` HTML helper takes an optional fallback message.

**Can I change the surrounding HTML (e.g., the DIV and SPAN elements) that is generated along with the reCAPTCHA?**
Not at this time. If there is sufficient interest, we can investigate using ASP.NET MVC's editor templates, or look for other means to support templating the HTML that's generated with reCAPTCHA. In the meantime, if this is critically important to you, implement your own [`ICaptchaGenerator`](https://github.com/NuGet/PoliteCaptcha/blob/master/PoliteCaptcha/ICaptchaGenerator.cs) and use [`ReCaptchaGenerator`](https://github.com/NuGet/PoliteCaptcha/blob/master/PoliteCaptcha/ReCaptchaGenerator.cs) as a starting-point.

_Ask questions not answered here by creating an issue._


using System;
using System.Reflection;
using System.Threading.Tasks;
using Acerva.Modelo;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;

namespace Acerva.Web
{
    public partial class Startup
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, Usuario>(
                        validateInterval: TimeSpan.FromMinutes(0),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
            }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");


            //var x = new FacebookAuthenticationOptions();
            //x.Scope.Add("email");
            //x.AppId = "230281167425626";
            //x.AppSecret = "64df738f15ed37c44a09e1e896fea6c5";
            //x.Provider = new FacebookAuthenticationProvider();
            ////{
            ////    OnAuthenticated = async context =>
            ////    {
            ////        //Get the access token from FB and store it in the database and
            ////        //use FacebookC# SDK to get more information about the user
            ////        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
            ////        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:name", context.Name));
            ////        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email));
            ////    }
            ////};
            //x.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            //app.UseFacebookAuthentication(x);

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = "230281167425626",
                AppSecret = "64df738f15ed37c44a09e1e896fea6c5",
                Scope = { "email" },
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                        return Task.FromResult(true);
                    }
                }
            });

            ////app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            ////{
            ////    ClientId = "512190296874-obl9munkkdcrdc66n1el94c0hd4rf4u1.apps.googleusercontent.com",
            ////    ClientSecret = "VHuiTjGaQmErKsZPGxgMd2py"
            ////});

            //var google = new GoogleOAuth2AuthenticationOptions()
            //{
            //    //ClientId = "512190296874-obl9munkkdcrdc66n1el94c0hd4rf4u1.apps.googleusercontent.com",
            //    ClientId = "629781104488-n3ri6e2nvs3oq205kijko9mc9gqeuaem.apps.googleusercontent.com",
            //    //ClientSecret = "peIjjjoTHSDe7SVdgJ8FfODF",
            //    ClientSecret = "mweW1ueX8HpYUBjh2Frwa-dM",
            //    Provider = new GoogleOAuth2AuthenticationProvider()
            //};
            //google.Scope.Add("email");
            //app.UseGoogleAuthentication(google);
        }
    }
}
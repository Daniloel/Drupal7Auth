using Drupal7Auth.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drupal7Auth.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        LoginModel user;
        HttpClient client;
        const string URL_API = "https://colstrat.000webhostapp.com";
        const string LOGIN = "/api/user/login.json";
        const string LOGOUT = "/api/user/logout.json";
        const string TOKEN = "/api/user/token.json";
        const string CONNECT = "/api/system/connect.json";
        const string USER = "test";
        const string PASSWORD = "test";
        public LoginPage ()
		{
			InitializeComponent ();
            btnLogin.Clicked += BtnLogin_Clicked;
            eUser.Text = USER;
            ePass.Text = PASSWORD;
            client = new HttpClient
            {
                BaseAddress = new Uri(URL_API),
                MaxResponseContentBufferSize = 9999999,
                Timeout = TimeSpan.FromSeconds(40)
            };
            btnGetConnect.IsEnabled = false;
            btnGetToken.IsEnabled = false;
            btnLogout.IsEnabled = false;
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(eUser.Text)) //if the username field is Null or Empty display a message
            {
                await DisplayAlert("Error", "Please write an username", "Ok");
                eUser.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ePass.Text)) //if the password field is Null or Empty display a message
            {
                await DisplayAlert("Error", "Please write a password", "Ok");
                ePass.Focus();
                return;
            }

            btnLogin.IsEnabled = false;
            string result;
            try
            {

                var httpContent = new HttpRequestMessage(HttpMethod.Post, LOGIN);

                // NOTE: I think there is no need for this
                httpContent.Headers.Add("Accept", "application/json");
                //httpContent.Headers.Add("Accept-Encoding", "gzip, deflate");

                // NOTE: Your server was returning 417 Expectation failed, this is set so the request client doesn't expect 100 and continue.
                httpContent.Headers.ExpectContinue = false;

                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", eUser.Text));
                values.Add(new KeyValuePair<string, string>("password", ePass.Text));

                httpContent.Content = new FormUrlEncodedContent(values);

                HttpResponseMessage response = await client.SendAsync(httpContent);

                // here is the hash as string
                result = await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException hre)
            {
                btnLogin.IsEnabled = true;
                return;
            }

            await DisplayAlert("Response", result, "OK");
            if (string.IsNullOrEmpty(result) || result == "[\"CSRF validation failed\"]" || result == "[\"Wrong username or password.\"]")
            {
                await DisplayAlert("Error", "Username or password incorrect", "OK");
                ePass.Text = string.Empty;
                ePass.Focus();
                return;
            }

            var deviceUser = JsonConvert.DeserializeObject<LoginModel>(result);
            user = deviceUser;

            Init();
        }
        private void Init()
        {
            userWelcome.Text = user.user.name;
            userToken.Text = user.token;
            userSessid.Text = user.sessid;
            btnLogout.IsEnabled = true;
            btnGetToken.IsEnabled = true;
            btnGetConnect.IsEnabled = true;
            btnGetConnect.Clicked += async (sender, e) => { await BtnGetConnect_Clicked(); };
            btnGetToken.Clicked += async (sender, e) => { await BtnGetToken_Clicked(); };
            btnLogout.Clicked += BtnLogout_Clicked;
        }

        private async void BtnLogout_Clicked(object sender, EventArgs e)
        {
            btnLogout.IsEnabled = false;
            btnGetToken.IsEnabled = false;
            btnGetConnect.IsEnabled = false;
            string result;
            try
            {
                HttpContent str = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");

                str.Headers.Add("Cookie", user.sessid);
                str.Headers.Add("X-CSRF-Token", user.token);
                var response = await client.PostAsync(new Uri(LOGOUT), str);
                result = response.Content.ReadAsStringAsync().Result;

            }
            catch (Exception err)
            {
                btnLogout.IsEnabled = true;
                btnGetToken.IsEnabled = true;
                btnGetConnect.IsEnabled = true;
                await DisplayAlert("Error", "Problem to connect, try later" + err, "OK");
                return;

            }
            await DisplayAlert("Response", result, "OK");

            btnLogin.IsEnabled = true;
        }

        private async Task<string> BtnGetToken_Clicked()
        {
            string token;
            try
            {
                HttpResponseMessage response = await client.PostAsync(TOKEN, null);

                // here is the hash as string
                token = await response.Content.ReadAsStringAsync();

            }
            catch (Exception err)
            {

                await DisplayAlert("Error", "Problem to connect, try later" + err, "OK");
                return "";
            }



            var tokenString = JsonConvert.DeserializeObject<Token>(token);
            userToken.Text = tokenString.token;
            user.token = tokenString.token;
            await DisplayAlert("Respuesta", token, "OK");

            return tokenString.token;
        }

        private async Task<Connect> BtnGetConnect_Clicked()
        {
            string connect;
            try
            {
                var httpContent = new HttpRequestMessage(HttpMethod.Post,CONNECT);

                // NOTE: I think there is no need for this
                //httpContent.Headers.Add("Accept", "application/json");
                //httpContent.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpContent.Headers.Add("X-CSRF-Token", user.token);
                // NOTE: Your server was returning 417 Expectation failed, this is set so the request client doesn't expect 100 and continue.
                httpContent.Headers.ExpectContinue = false;

                /*var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("X-CSRF-Token", user.token));

                httpContent.Content = new FormUrlEncodedContent(values);*/

                HttpResponseMessage response = await client.SendAsync(httpContent);

                // here is the hash as string
                connect = await response.Content.ReadAsStringAsync();

            }
            catch (Exception err)
            {
                
                await DisplayAlert("Error", "Problem to connect, try later" + err, "OK");
                return null;
            }
            if (string.IsNullOrEmpty(connect) || connect == "[\"CSRF validation failed\"]")
            {
                await DisplayAlert("Error", "CSRF validation failed", "OK");
                return null;
            }
            var connectResult = JsonConvert.DeserializeObject<Connect>(connect);
            user.sessid = connectResult.sessid;
            userSessid.Text = connectResult.sessid;
            await DisplayAlert("Respuesta", connect, "OK");

            return connectResult;
        }
    }
}
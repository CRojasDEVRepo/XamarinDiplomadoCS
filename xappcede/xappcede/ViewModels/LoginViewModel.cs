using Acr.UserDialogs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace xappcede.ViewModels
{
    public class LoginViewModel
    {
        #region Private attributes
        private string _email;
        private string _password;
        private INavigation _inavigation { get; set; }
        #endregion
        #region Public attributes
        public ICommand LoginCommand { get; set; }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        #endregion
        #region Constructor
        public LoginViewModel(INavigation navegation)
        {
            _inavigation = navegation;
            LoginCommand = new Command(async () => await Login());
        }

        #endregion
        #region Methods
        private async Task Login()
        {
            bool isvalid = await ValidateLogin();

            if (isvalid)
            {
                var loginModel = new
                {
                    Email = Email,
                    Password = Password
                };
                HttpResponseMessage httpResponse = await PostLogin(loginModel);

                if (!httpResponse.IsSuccessStatusCode)
                    await UserDialogs.Instance.AlertAsync("Please, Try again later", "Error");
                else
                {
                    string c = await httpResponse.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject(c);
                    await UserDialogs.Instance.AlertAsync(resultado.ToString(), "Welcome");
                }
            }
        }

        private async Task<bool> ValidateLogin()
        {
            if (Password == string.Empty || Email == string.Empty)
            {
                await UserDialogs.Instance.AlertAsync("Email o contraseña erroneos", "Error");
                return false;
            }
            else
                return true;
        }

        private async Task<HttpResponseMessage> PostLogin(object loginModel)
        {
            string json = JsonConvert.SerializeObject(loginModel);
            HttpClient httpClient = new HttpClient();
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("https://ecommercebackendapi.azurewebsites.net/api/Accounts/Login", content);

            return response;
        }
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MqttChatClient.ViewModels
{
    public class LoginViewModel: ContactBase
    {
        #region Fields

        private string phoneNumber;
        private string password;
        private bool isLoginButtonEnabled;
        private bool isRegisterButtonEnabled;

        #endregion Fields

        #region Properties

        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        public bool IsLoginButtonEnabled
        {
            get
            {
                return isLoginButtonEnabled;
            }
            set
            {
                isLoginButtonEnabled = value;
                RaisePropertyChanged("IsLoginButtonEnabled");
            }
        }

        public bool IsRegisterButtonEnabled
        {
            get
            {
                return isRegisterButtonEnabled;
            }
            set
            {
                isRegisterButtonEnabled = value;
                RaisePropertyChanged("IsRegisterButtonEnabled");
            }
        }

        public ICommand SubmitCommand { protected set; get; }
        public ICommand RegisterCommand{ protected set; get; }

        public INavigation Navigation { get; set; }

        public Action DisplayInvalidLoginPrompt;
        public Action RegistrationNotAvailable;

        #endregion Properties

        #region Constructors

        public LoginViewModel(INavigation navigation)
        {
            Navigation = navigation;
            IsLoginButtonEnabled = true;
            IsRegisterButtonEnabled = true;
            SubmitCommand = new Command(OnSubmit);
            RegisterCommand = new Command(Register);
        }

        #endregion Constructors

        #region Methods

        public void OnSubmit()
        {
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(password))
            {
                DisplayInvalidLoginPrompt();
            }
            else
            {
                //App.Instance.Properties["IsLoggedIn"] = true;
                IsLoginButtonEnabled = false;
                App.Instance.ProceedWithLogin(PhoneNumber, Navigation);
            }
        }

        public void Register()
        {
            IsRegisterButtonEnabled = false;
            RegistrationNotAvailable();
        }

        #endregion Methods
    }
}

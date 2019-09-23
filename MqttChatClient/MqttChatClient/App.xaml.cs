using System;
using System.IO;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MqttChatClient.Models;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MqttChatClient
{
    public partial class App : Application
    {
        #region CONST

        private const string BROKER_HOST_NAME = "ACA";

        #endregion CONST

        #region Fields

        private static DataBase _appDataBase;

        #endregion Fields

        #region Properties

        private MqttClient mqttClient;

        public string PhoneNumber { get; set; }

        public IEnumerable<PhoneContact> Contacts { get; set; }

        public static X509Certificate Cert { get; set; }

        public static DataBase AppDataBase
        {
            get
            {
                if (_appDataBase == null)
                    _appDataBase = new DataBase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MqttChatSQLite.db3"));
                return _appDataBase;
            }
        }

        public static App Instance
        {
            get { return Current as App; }
        }

        public bool ShouldSubscribe { get; set; }

        #endregion Properties

        #region Constructors

        public App()
        {
            InitializeComponent();
            Contacts = new List<PhoneContact>();
            PhoneNumber = string.Empty;
            //Current.Properties["IsLoggedIn"] = false;
            
            MainPage = new NavigationPage(new LoginPage())
            {
                BarTextColor = Color.Azure,
                BarBackgroundColor = Color.FromHex("#17445e")
            };
             // Client should subscribe only once when registering
             ShouldSubscribe = true;
        }

        #endregion Constructors

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region Methods

        public void ProceedWithLogin(string phoneNumber, INavigation navigation)
        {
            navigation.PushAsync(new HomePage());
            PhoneNumber = phoneNumber;
        }

        public void InitializeWorkingResources()
        {
            Resource r = new Resource();
            Cert = new X509Certificate(Resource.ca);
            Contacts = DependencyService.Get<IContactService>().GetAllContacts();
            PhoneNumber = DependencyService.Get<IContactService>().GetCurrentPhoneNumberCorrectFormat(PhoneNumber).Trim();

            InitializeMQTTClient();
            if (mqttClient.IsConnected && !string.IsNullOrEmpty(PhoneNumber))
            {
                MessagingCenter.Send(this, "resourcesInitialized");
            }
        }

        private void InitializeMQTTClient()
        {
            mqttClient = new MqttClient(BROKER_HOST_NAME, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, Cert, null, MqttSslProtocols.TLSv1_2, MyRemoteCertificateValidationCallback);

            // SEND LOGIN DATA -> USER NAME AND PASSWORD
            mqttClient.Connect(PhoneNumber, null, null, false, 2000);

            if (mqttClient.IsConnected)
            {
                mqttClient.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

                if (ShouldSubscribe)
                {
                    string topic = string.Format("{0}/#", PhoneNumber);
                    mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string receivedMessage = Encoding.Default.GetString(e.Message);
            string[] topicLevels = e.Topic.Split('/');

            if (topicLevels.Length == 2)
            {
                string messageSender = topicLevels[1];
                Message message = new Message()
                {
                    Text = receivedMessage,
                    Sender = messageSender,
                    Receiver = PhoneNumber,
                    CreatedTime = DateTime.Now,
                    Status = MessageStatus.NotRead
                };
                AppDataBase.SaveItemAsync(message);
                MessagingCenter.Send(this, "newMessageReceived", message);
            }
        }

        public void PublishNewMessageAsync(Message message)
        {
            string topic = string.Format("{0}/{1}", message.Receiver, PhoneNumber);
            byte[] data = Encoding.UTF8.GetBytes(message.Text);
            if (mqttClient.IsConnected)
            {
                mqttClient.Publish(topic, data, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                AppDataBase.SaveItemAsync(message);
            }
        }

        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else if (certificate.Issuer.Equals(Cert.Issuer)) // WORKAROUND APPLIED BECAUSE CHAINSTATUS IS NOT IMPLEMENTED
                return true;
            else
                return false;
        }

        #endregion Methods
    }
}

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MqttChatClient
{
    public partial class App : Application
    {
        #region CONST

        public const string ONLINE = "Online";

        #endregion CONST

        List<Friend> FriendList = new List<Friend>() { new Friend() { Name = "Friend1"}, new Friend() { Name = "Friend2" }, new Friend() { Name = "Friend3" } };

        #region Properties

        private MqttClient mqttClient;

        public MainPage mainPage;

        public List<MessagingPage> MessagingPages = new List<MessagingPage>();

        #endregion Properties

        #region Constructors

        public App()
        {
            InitializeComponent();
            InitializeMQTTClient();
            mainPage = new MainPage();
            mainPage.CreateFriendList(FriendList);
            MainPage = new NavigationPage(mainPage);
        }

        #endregion Constructors

        protected override void OnStart()
        {    // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


        public static App Instance
        {
            get { return Current as App; }
        }

        private void InitializeMQTTClient()
        {
            mqttClient = new MqttClient("192.168.100.193");
            string clientId = "Alex";
            // will parameters
            string userOnlineTopic = string.Format("{0}/Online", clientId);

            mqttClient.Connect(clientId, null, null, true, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true, userOnlineTopic, "false", false, 1000);

            //subscribe to topics
            if (mqttClient.IsConnected)
            {
                List<string> friendUserTopics = FriendList.Select(f => f.Name + "/Alex/#").ToList();
                List<string> friendOnlineTopics = FriendList.Select(f => f.Name + "/Online").ToList();
                string[] topics = friendUserTopics.Concat(friendOnlineTopics).ToArray();
                byte[] qosLevels = Enumerable.Repeat(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, topics.Length).ToArray();
                mqttClient.Subscribe(topics, qosLevels);

                byte[] userOnlineData = Encoding.UTF8.GetBytes("true");
                mqttClient.Publish(userOnlineTopic, userOnlineData, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }

            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string receivedMessage = Encoding.Default.GetString(e.Message);
            string[] topicLevels = e.Topic.Split('/');

            if (topicLevels.Length == 1)
                return;

            Friend messagingFriend = FriendList.Find(f => f.Name.Equals(topicLevels[0]));
            if (messagingFriend == null)
                return;

            MessagingPage messagingPage = MessagingPages.Find(page => page.MessageFriend.Name.Equals(topicLevels[0]));
            if (messagingPage == null)
            {
                messagingPage = new MessagingPage(messagingFriend);
                MessagingPages.Add(messagingPage);
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (topicLevels[1].Equals(ONLINE))
                    {
                        messagingPage.MessageFriend.Status = receivedMessage.Equals("true") ? "online" : "offline";
                        mainPage.ChangeFriendsStatus(messagingPage.MessageFriend); 
                    }
                    else if (topicLevels[2].Equals("Message"))
                    {
                        mainPage.ChangeFriendsNameColor(messagingFriend, true);
                        messagingPage.DisplayNewMessage(receivedMessage);
                    }
                    else if (topicLevels[2].Equals("AreFriends"))
                    {
                        bool areFriends = receivedMessage.Equals("true") ? true : false;
                        if (!areFriends)
                        {
                            mainPage.RemoveFriend(messagingFriend);
                            UnsubscribeFromFriend(messagingFriend);        
                        }

                    }
                }
                catch (Exception)
                {
                  //  DisplayAlert("error", ex.Message, "ok");
                }
            });
        }

        public void RemoveFriend(Friend friend)
        {
            PublishNewMessage(friend, "AreFriends", "false", true);
            UnsubscribeFromFriend(friend);
            mainPage.RemoveFriend(friend);
            // remove from messaging pages
        }

        public void PublishNewMessage(Friend friend, string topicPart, string message, bool retain)
        {
            string topic = FriendUserThreeLevelTopic(friend, topicPart);
            byte[] data = Encoding.UTF8.GetBytes(message);
            mqttClient.Publish(topic, data, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, retain);
        }

        private string FriendUserTwoLevelTopic(Friend friend)
        {
            string result = string.Format("{0}/{1}", mqttClient.ClientId, friend.Name);
            return result;
        }

        private string FriendUserThreeLevelTopic(Friend friend, string thirdLevel)
        {
            string twoLevel = FriendUserTwoLevelTopic(friend);
            string result = string.IsNullOrEmpty(thirdLevel) ? twoLevel : twoLevel + "/" + thirdLevel;
            return result;
        }

        private void UnsubscribeFromFriend(Friend friend)
        {
            string onlineTopic = string.Format("{0}/Online", friend.Name);
            string friendTopics = string.Format("{0}/#", friend.Name);
            mqttClient.Unsubscribe(new string[] { friendTopics });
            FriendList.Remove(friend);
        }
    }
}

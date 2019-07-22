using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MqttChatClient
{
    public partial class MainPage : ContentPage
    {
        #region Fields

        private ObservableCollection<Friend> _friends;

        #endregion Fiels

        #region Properties

        public ObservableCollection<Friend> Friends
        {
            get
            {
                if (_friends == null)
                    _friends = new ObservableCollection<Friend>();
                return _friends;
            }
            set
            {
                _friends = value;
            }
        }

        #endregion Properties

        #region Constructors 

        public MainPage()
        {
            InitializeComponent();      
        }

        #endregion Constructors

        #region Methods 

        public void CreateFriendList(List<Friend> friendList)
        {
            Friends = new ObservableCollection<Friend>(friendList);

            if (Friends.Count == 0)
            {
                Content = new Label
                {
                    Text = "Your friend list is empty"
                }; 
            }
            else
            {
                ListView list = new ListView
                {
                    AutomationId = "FriendList",
                    Header = "Friend list",
                    ItemTemplate = new DataTemplate(typeof(ImageCell))
                    {
                        Bindings = {
                            { ImageCell.TextProperty, new Binding("Name") },
                            {ImageCell.TextColorProperty, new Binding("NameColor") },
                            {ImageCell.DetailProperty, new Binding("Status") },
                            {ImageCell.DetailColorProperty, new Binding("StatusColor") }
                        }
                    },
                    ItemsSource = Friends,
                };
                list.ItemTapped += ItemTapped;
                Content = list;
            }
        }

        public void ChangeFriendsStatus(Friend friend)
        {
            Friend friendInList = Friends.First(f => f.Name.Equals(friend.Name));
            if (friend != null)
                friendInList.Status = friend.Status;
        }

        public void ChangeFriendsNameColor(Friend friend, bool unread)
        {
            Friend friendInList = Friends.First(f => f.Name.Equals(friend.Name));
            if (friend != null)
            {
                if (unread)
                    friendInList.SetBlackNameColor();
                else
                    friendInList.SetGrayNameColor();
            }
        }

        public void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var myListView = (ListView)sender;
            Friend myItem = (Friend)myListView.SelectedItem;

            MessagingPage mPage = App.Instance.MessagingPages.Find(page => page.MessageFriend.Equals(myItem));
            if (mPage == null)
            {
                mPage = new MessagingPage(myItem);
                App.Instance.MessagingPages.Add(mPage);
            }
            Navigation.PushAsync(mPage);
            ChangeFriendsNameColor(myItem, false);
        }

        public void RemoveFriend(Friend friend)
        {
            Friends.Remove(friend);
        }

        #endregion Methods
    }
}

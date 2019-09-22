using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Android.Provider;
using MqttChatClient.Droid;
using Android.Telephony;
using PhoneNumbers;
using Application = Android.App.Application;
using System.IO;

[assembly: Dependency(typeof(ContactServiceImplementation))]
namespace MqttChatClient.Droid
{
    public class ContactServiceImplementation : IContactService
    {
       public IEnumerable<PhoneContact> GetAllContacts()
        {
            var phoneContacts = new List<PhoneContact>();

            using (var phonesCursor = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
            {
                if (phonesCursor != null)
                {
                    while (phonesCursor.MoveToNext())
                    {
                        try
                        {
                            // consider putting country code
                            // number should to be in +.. format
                            string name = phonesCursor.GetString(phonesCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                            string phoneNumber = phonesCursor.GetString(phonesCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                            string imageUri = phonesCursor.GetString(phonesCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.PhotoUri));

                            PhoneContact contact = new PhoneContact();

                            string[] firstAndLastName = name.Split(' ');
                            contact.FirstName = firstAndLastName[0];
                            if (firstAndLastName.Length > 1)
                                contact.LastName = firstAndLastName[1];
                            else
                                contact.LastName = string.Empty;
                            
                            var phoneUtil = PhoneNumberUtil.GetInstance();
                            var numberProto = phoneUtil.Parse(phoneNumber, "IT");
                            string formattedPhone = phoneUtil.Format(numberProto, PhoneNumbers.PhoneNumberFormat.INTERNATIONAL);
                            contact.PhoneNumber = string.IsNullOrEmpty(formattedPhone) ? string.Empty : Regex.Replace(formattedPhone, "[+ ]", string.Empty);
                            var stream = string.IsNullOrEmpty(imageUri) ? null : Application.Context.ContentResolver.OpenInputStream(Android.Net.Uri.Parse(imageUri));
                            if (stream == null)
                                contact.ImageSource = ImageSource.FromFile("UnknownPerson");
                            else
                            {
                                byte[] imageBytes = ReadFully(stream);
                                contact.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                            }
                            var list = phoneContacts.Where(c => c.PhoneNumber.Equals(phoneNumber));
                            if (list == null || list.Count() == 0)
                                phoneContacts.Add(contact);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    phonesCursor.Close();
                }
            }

            return phoneContacts;
        }

       public string GetCurrentPhoneNumberCorrectFormat(string phoneNumber)
        {
            // Logic with TelephonyManager doesnt always work
            //TelephonyManager mgr = Application.Context.GetSystemService(Context.TelephonyService) as TelephonyManager;
            //string phoneNumber = mgr.Line1Number;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                // consider putting country code
                // number should to be in +.. format
                var numberProto = phoneUtil.Parse(phoneNumber, "IT");
                string formattedPhone = phoneUtil.Format(numberProto, PhoneNumbers.PhoneNumberFormat.INTERNATIONAL);
                phoneNumber = string.IsNullOrEmpty(formattedPhone) ? string.Empty : Regex.Replace(formattedPhone, "[+ ]", string.Empty);
            }
            return phoneNumber;
        }

       public void ShowPopUpForCurrentPhoneNumber(PopupEntry pe)
        {
            var alert = new AlertDialog.Builder(MainActivity.ThisActivity);

            var edit = new EditText(MainActivity.ThisActivity) { Text = pe.Text };
            edit.InputType = Android.Text.InputTypes.ClassPhone;
            alert.SetView(edit);

            alert.SetTitle(pe.Title);

            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                pe.OnPopupClosed(new PopupEntryClosedArgs
                {
                    Button = "OK",
                    Text = edit.Text
                });
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                pe.OnPopupClosed(new PopupEntryClosedArgs
                {
                    Button = "Cancel",
                    Text = edit.Text
                });
            });
            alert.Show();
        }

       public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
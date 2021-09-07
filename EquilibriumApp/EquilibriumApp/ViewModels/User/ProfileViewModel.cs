using Acr.UserDialogs;
using EquilibriumApp.Helpers;
using EquilibriumApp.Mobile.Extentions;
using EquilibriumApp.Mobile.Helpers;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using EquilibriumApp.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.ViewModels.User
{
    public class ProfileViewModel : BaseViewModel
    {
        private string _imageSource;

        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        private AccountUserModel _user;

        public AccountUserModel User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }


        private string _profileUrl;

        public string ProfileUrl
        {
            get { return _profileUrl; }
            set { SetProperty(ref _profileUrl, value); }
        }


        private readonly IFileClient _fileClient;

        public ProfileViewModel(IFileClient fileClient)
        {
            _fileClient = fileClient;
            RefreshDataCommand.Execute(null);
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        private ICommand _refreshDataCommand;

        public ICommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand ?? (_refreshDataCommand = new Command(async () =>
                {
                    IsBusy = true;
                    try
                    {
                        User = await AuthenticationService.GetUserInformation();
                        if (User.IsImage)
                        {
                            ProfileUrl = User.ProfileUrl;
                        }
                        else
                        {
                            if (User.Gender == 0)
                            {
                                ProfileUrl = "https://lmsblob.blob.core.windows.net/profile-images/person-male.png";
                            }
                            else if (User.Gender == 1)
                            {
                                ProfileUrl = "https://lmsblob.blob.core.windows.net/profile-images/person-female.png";
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                   
                    IsBusy = false;
                }));
            }
        }

        private ICommand _signOutCommand;

        public ICommand SgnOutCommand
        {
            get
            {
                return _signOutCommand ?? (_signOutCommand = new Command(async () =>
                {
                    try
                    {
                        AuthenticationService.Logout();
                        await _navigationService.NavigateBackModalAsync<ProfileViewModel>(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    IsBusy = false;
                }));
            }
        }

        private ICommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new Command(async () =>
                {
                    try
                    {
                        User.Email = User.Email?.Trim();
                        if (!ValidationHelper.ValidateEmail(User.Email))
                        {
                            await UserDialogs.Instance.AlertAsync("Email is invalid");
                            return;
                        }
                        User.PhoneNumber = User.PhoneNumber?.Trim();
                        if (!ValidationHelper.ValidatePhoneNumber(User.PhoneNumber))
                        {
                            UserDialogs.Instance.Toast("Phone number is invalid, please remove the leading 0 and ensure full number entered.", TimeSpan.FromSeconds(5));
                            return;
                        }

                        if (DateTime.Now.AddYears(-10) < User.DateOfBirth)
                        {
                            UserDialogs.Instance.Toast("Please check your selected date of birth.", TimeSpan.FromSeconds(5));
                            return;
                        }

                        IsBusy = true;
                        if (!string.IsNullOrEmpty(User.ProfileBase64))
                        {
                            var fileModel = new FileModel()
                            {
                                Base64 = User.ProfileBase64,
                                Name = Path.GetFileName(User.ProfileUrl),
                                Size = 10
                            };
                            var uploadModel = await _fileClient.PostFileAsync(fileModel);
                            User.ProfileUrl = uploadModel?.Path;
                        }

                        var result = await AuthenticationService.PostUserInformation(User);
                        if (result)
                        {
                            IsBusy = false;
                            MessagingCenter.Send<Messages>(new Messages(), "update_bookings");
                            await UserDialogs.Instance.AlertAsync("Profile updated successfully");
                            await _navigationService.NavigateBackModalAsync<ProfileViewModel>(true);
                        }
                        else
                        {
                            await UserDialogs.Instance.AlertAsync("Error updating user profile.");
                        }
                    }
                    catch (Exception e)
                    {
                        await UserDialogs.Instance.AlertAsync("Error updating user profile.");
                       
                    }
                    finally
                    {
                        IsBusy = false;
                    }

                 
                }));
            }
        }

        private ICommand _cameraCommand;

        public ICommand CameraCommand
        {
            get
            {
                return _cameraCommand ?? (_cameraCommand = new Command(async () =>
                {
                    try
                    {
                        if (CrossMedia.IsSupported && CrossMedia.Current.IsPickPhotoSupported &&
                      CrossMedia.Current.IsTakePhotoSupported)
                        {
                            var dialog = await UserDialogs.Instance.ActionSheetAsync("Update profile image", "Cancel",
                                string.Empty, CancellationToken.None, new[] { "Use Camera", "Choose from photo album" });
                            if (dialog == "Use Camera")
                            {
                                if (await GeneralHelper.RequestPermission(Permission.Camera))
                                {

                                    var ready = await CrossMedia.Current.Initialize();
                                    if (!ready) return;
                                    var media = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                                    {
                                        PhotoSize = PhotoSize.MaxWidthHeight,
                                        MaxWidthHeight = 500
                                    });
                                    if (media == null) return;
                                    ImageSource = new FileImageSource() { File = media.Path };
                                    User.ProfileUrl = new FileImageSource() { File = media.Path };
                                    ProfileUrl = new FileImageSource() { File = media.Path };
                                    User.ProfileBase64 = Convert.ToBase64String(media.GetStream().ToByteArray());
                                    media.GetStream().Dispose();
                                }
                                else
                                {
                                    if (await UserDialogs.Instance.ConfirmAsync("Do you want allow permission from settings?", "Permission denied", "Yes", "No"))
                                    {
                                        CrossPermissions.Current.OpenAppSettings();
                                    }


                                }

                            }
                            else if (dialog == "Choose from photo album")
                            {
                                if (await GeneralHelper.RequestPermission(Permission.Photos))
                                {
                                    var ready = await CrossMedia.Current.Initialize();
                                    if (!ready) return;
                                    var media = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                                    {
                                        PhotoSize = PhotoSize.MaxWidthHeight,
                                        MaxWidthHeight = 500
                                    });
                                    if (media == null) return;
                                    ImageSource = new FileImageSource() { File = media.Path };
                                    User.ProfileUrl = new FileImageSource() { File = media.Path };
                                    ProfileUrl = new FileImageSource() { File = media.Path };
                                    User.ProfileBase64 = Convert.ToBase64String(media.GetStream().ToByteArray());
                                    media.GetStream().Dispose();
                                }
                                else
                                {

                                    if (await UserDialogs.Instance.ConfirmAsync("Do you want allow permission from settings?", "Permission denied", "Yes", "No"))
                                    {
                                        CrossPermissions.Current.OpenAppSettings();
                                    }
                                }



                            }
                        }
                    }
                    catch (Exception)
                    {


                    }

                }));
            }
        }

        public List<string> Genders
        {
            get
            {
                return Enum.GetNames(typeof(Gender)).Select(b => b).ToList();
            }
        }
    }
}
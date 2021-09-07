using EquilibriumApp.Controls.XamlControls.ViewModels;
using EquilibriumApp.DI;
using EquilibriumApp.Mobile.Models.ViewModels;
using EquilibriumApp.Models.ViewModels;
using RX.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Controls.XamlControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavouriteControl : ContentView
    {
     

        public static readonly BindableProperty FavouriteClassIdProperty = BindableProperty.Create(
                                                        propertyName: "FavouriteClassId",
                                                        returnType: typeof(int?),
                                                        declaringType: typeof(FavouriteControl),
                                                        defaultValue: null,
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        null,
                                                        (bindable, value, newValue) =>
                                                        {
                                                            var control = (FavouriteControl)bindable;
                                                            control.FavouriteDataCommand.Execute(null);
                                                        });
        public int? FavouriteClassId
        {
            get {  
                    if (base.GetValue(FavouriteClassIdProperty) != null)
                    {
                        return Convert.ToInt32(base.GetValue(FavouriteClassIdProperty));
                    }
                    return null;
                }  
            set { 
                base.SetValue(FavouriteClassIdProperty, value);
            }
        }


        public static readonly BindableProperty FavouriteFacilityIdProperty = BindableProperty.Create(
                                                        propertyName: "FavouriteFacilityId",
                                                        returnType: typeof(int?),
                                                        declaringType: typeof(FavouriteControl),
                                                        defaultValue: null,
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        null,
                                                        (bindable, value, newValue) =>
                                                        {
                                                            var control = (FavouriteControl)bindable;
                                                            control.FavouriteDataCommand.Execute(null);
                                                        });
        public int? FavouriteFacilityId
        {
            get
            {
                if (base.GetValue(FavouriteFacilityIdProperty) != null)
                {
                    return Convert.ToInt32(base.GetValue(FavouriteFacilityIdProperty));
                }
                return null;
            }
            set
            {
                base.SetValue(FavouriteClassIdProperty, value);
            }
        }

        private UserFavouriteModel userFavourite { get; set; }


        private readonly IFavouriteClient _favouriteClient;

        public FavouriteControl()
        {
            InitializeComponent();
            _favouriteClient = (IFavouriteClient)Locator.Instance.Resolve(typeof(IFavouriteClient));
        }

        private ICommand _favouriteDataCommand;

        public ICommand FavouriteDataCommand
        {
            get
            {
                return _favouriteDataCommand ?? (_favouriteDataCommand = new Command(async () =>
                {
                    try
                    {
                        userFavourite = await _favouriteClient.GetFavouriteAsync(null, FavouriteClassId, FavouriteFacilityId);
                        if (userFavourite == null)
                        {
                            FavouriteButton.ImageSource = "favourite.png";
                        }
                        else
                        {
                            FavouriteButton.ImageSource = "favourite_filled.png";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }));
            }
        }

        private ICommand _favouriteCommand;

        public ICommand FavouriteCommand
        {
            get
            {
                return _favouriteCommand ?? (_favouriteCommand = new Command(async () =>
                {
                    if (userFavourite == null)
                    {
                        var result = await _favouriteClient.AddFavouriteAsync(null, FavouriteClassId, FavouriteFacilityId);
                    }
                    else
                    {
                        await _favouriteClient.RemoveFavouriteAsync(null, FavouriteClassId, FavouriteFacilityId);
                    }
                    MessagingCenter.Send<Messages>(new Messages(), "update_favourites");
                    FavouriteDataCommand.Execute(null);
                }));
            }
        }

        void FavouriteButton_Clicked(object sender, EventArgs e)
        {
            FavouriteCommand.Execute(null);
        }
    }
}
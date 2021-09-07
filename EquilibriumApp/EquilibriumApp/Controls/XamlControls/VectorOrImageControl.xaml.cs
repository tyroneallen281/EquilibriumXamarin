using System.Collections.Generic;
using System.Diagnostics;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using ImageCircle.Forms.Plugin.Abstractions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EquilibriumApp.Mobile.Controls.XamlControls
{
    public enum ImageTypeEnum
    {
        Circle,
        Square
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VectorOrImageControl : ContentView
    {
        const float TextOffset = 1.25f;
        
        private CachedImage _cachedImageIos = new CachedImage()
        {
            CacheType = CacheType.Disk,
            RetryCount = 5,
            Transformations = new List<ITransformation>() { new CircleTransformation() }
        };
        
        private CachedImage _cachedImageAndroid = new CachedImage()
        {
            CacheType = CacheType.Disk,
            RetryCount = 5,
            Transformations = new List<ITransformation>() { new CircleTransformation()}
        };

        public VectorOrImageControl()
        {
            InitializeComponent();
        }


        public static readonly BindableProperty ImageTypeProperty = BindableProperty.Create("ImageType",
            typeof(ImageTypeEnum), typeof(VectorOrImageControl), ImageTypeEnum.Circle, BindingMode.OneWay, null,
            (bindable, value, newValue) =>
            {

                var control = (VectorOrImageControl)bindable;
                switch (control.ImageType)
                {
                    case ImageTypeEnum.Circle:
                        control._cachedImageAndroid.Transformations =
                            new List<ITransformation>() {new CircleTransformation()};
                        control._cachedImageIos.Transformations =
                            new List<ITransformation>() {new CircleTransformation()};
                        control._cachedImageAndroid.ReloadImage();
                        control._cachedImageIos.ReloadImage();
                        break;
                    case ImageTypeEnum.Square:
                        control._cachedImageAndroid.Transformations =
                            new List<ITransformation>() {new RoundedTransformation(40)};
                        control._cachedImageIos.Transformations =
                            new List<ITransformation>() {new RoundedTransformation(40)};
                        control._cachedImageAndroid.ReloadImage();
                        control._cachedImageIos.ReloadImage();
                        break;
                }
            });

        public ImageTypeEnum ImageType
        {
            get { return (ImageTypeEnum)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }

        public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create("LabelFontSize",
            typeof(double), typeof(VectorOrImageControl), double.MinValue, BindingMode.OneWay, null,
            (bindable, value, newValue) =>
            {
                var control = (VectorOrImageControl) bindable;
               
            });

        public double LabelFontSize
        {
            get { return (double) GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        public static readonly BindableProperty ContentColorProperty = BindableProperty.Create("ContentColor",
            typeof(Color), typeof(VectorOrImageControl), Color.Default, BindingMode.OneWay, null,
            (bindable, value, newValue) =>
            {
                var control = (VectorOrImageControl) bindable;
               
            });

        public Color ContentColor
        {
            get { return (Color) GetValue(ContentColorProperty); }
            set { SetValue(ContentColorProperty, value); }
        }

        public static readonly BindableProperty ImageUrlProperty = BindableProperty.Create("ImageUrl", typeof(string),
            typeof(VectorOrImageControl), string.Empty, BindingMode.TwoWay, null,
            (bindable, value, newValue) =>
            {
                var view = (bindable as VectorOrImageControl);
                view._cachedImageIos.Error += _cachedImageIos_Error;
                var imageUrl = newValue as string;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            if (view != null)
                            {
                                view._cachedImageAndroid.Source = imageUrl;
                                view._cachedImageAndroid.ReloadImage();
                               
                               
                            }
                            break;
                        case Device.iOS:
                            if (view != null)
                            {
                                view._cachedImageIos.Source = imageUrl;
                                view._cachedImageIos.ReloadImage();
                            }
                            break;

                    }

                    view?.ChooseView(true);
                }



            });

        private static void _cachedImageIos_Error(object sender, CachedImageEvents.ErrorEventArgs e)
        {
           
        }

        public string ImageUrl
        {
            get { return (string) GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public static readonly BindableProperty IsImageProperty = BindableProperty.Create("IsImage", typeof(bool),
            typeof(VectorOrImageControl), true, BindingMode.TwoWay, null,
            propertyChanging: (bindable, value, newValue) =>
            {

                var view = (bindable as VectorOrImageControl);
                view?.ChooseView((bool) newValue);

            });

        public bool IsImage
        {
            get { return (bool) GetValue(IsImageProperty); }
            set
            {
                SetValue(IsImageProperty, value);
                ChooseView(value);
            }
        }


        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string),
            typeof(VectorOrImageControl), string.Empty, BindingMode.TwoWay, null,
            (bindable, value, newValue) =>
            {
                var view = (bindable as VectorOrImageControl);
                 view._cachedImageAndroid.ReloadImage();
                view._cachedImageIos.ReloadImage();
            });

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public void ChooseView(bool isImage)
        {
            if (isImage)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        _cachedImageAndroid.HeightRequest = HeightRequest;
                        _cachedImageAndroid.WidthRequest = WidthRequest;
                        Content = _cachedImageAndroid;
                        break;
                    case Device.iOS:
                        _cachedImageIos.HeightRequest = HeightRequest;
                        _cachedImageIos.WidthRequest = WidthRequest;
                        Content = _cachedImageIos;
                        break;

                }


            }
           
        }

    }
}
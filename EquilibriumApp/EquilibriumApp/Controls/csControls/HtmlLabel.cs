using System;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace EquilibriumApp.Mobile.Controls.CsControls
{
   
    public class HtmlLabel : Label
    {

        public HtmlLabel()
        {
            this.LineBreakMode = LineBreakMode.WordWrap;
        }
        public static readonly BindableProperty ScrollEnabledProperty =
            BindableProperty.Create(nameof(ScrollEnabled), typeof(bool), typeof(HtmlLabel), false);

        public bool ScrollEnabled
        {
            get { return (bool)GetValue(ScrollEnabledProperty); }
            set { SetValue(ScrollEnabledProperty, value); }
        }

        public static readonly BindableProperty IsLongCharacterProperty =
            BindableProperty.Create(nameof(IsLongCharacter), typeof(bool), typeof(HtmlLabel), false, BindingMode.TwoWay);

       

        public bool IsLongCharacter
        {
            get { return (bool)GetValue(IsLongCharacterProperty); }
            set { SetValue(IsLongCharacterProperty, value); }
        }
        public static readonly BindableProperty IsLinkedEnabledProperty =
            BindableProperty.Create(nameof(IsLinkEnabled), typeof(bool), typeof(HtmlLabel), false);

        public bool IsLinkEnabled
        {
            get { return (bool)GetValue(IsLinkedEnabledProperty); }
            set { SetValue(IsLinkedEnabledProperty, value); }
        }


        public static readonly BindableProperty CustomTextProperty =
            BindableProperty.Create(nameof(CustomText), typeof(string), typeof(HtmlLabel), string.Empty);

        public string CustomText
        {
            get { return (string)GetValue(CustomTextProperty); }
            set { SetValue(CustomTextProperty, value); }
        }


        public void SetText(string text)
        {
            this.Text = text;
            this.IsLongCharacter = false;
            this.SetValue(TextProperty, text);
        }

        public static void SetMaxLines(BindableObject view, int value)
        {
            view?.SetValue(MaxLinesProperty, value);
        }

        /// <summary>
        /// Send the Navigating event
        /// </summary>
        /// <param name="args"></param>
        public void SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        /// <summary>
        /// Send the Navigated event
        /// </summary>
        /// <param name="args"></param>
        public void SendNavigated(WebNavigatingEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        /// <summary>
        /// Fires before the open URL request is done.
        /// </summary>
        public event EventHandler<WebNavigatingEventArgs> Navigating;

        /// <summary>
        /// Fires when the open URL request is done.
        /// </summary>
        public event EventHandler<WebNavigatingEventArgs> Navigated;

    }

    public class LabelRendererHelper
    {
        private readonly Label _label;
        private readonly bool _expand;
        private readonly string _text;
        private readonly StringBuilder _builder;

        public LabelRendererHelper(Label label, string text, bool expand = false)
        {
            _label = label;
            _expand = expand;
            _text = text.Trim();
            if (expand)
            {

               var stringBuilder = new StringBuilder(_text);
               stringBuilder.Append("</a></p>");

                stringBuilder.Append("<a href=\"http://www.readmore.com\"> ... read more </a>");
                _text = stringBuilder.ToString();
            }
            _builder = new StringBuilder();
        }

        private void SetFontAttributes()
        {
            if (_label.FontAttributes == FontAttributes.None) return;
            switch (_label.FontAttributes)
            {
                case FontAttributes.Bold:
                    _builder.Append("font-weight: bold; ");
                    break;
                case FontAttributes.Italic:
                    _builder.Append("font-style: italic; ");
                    break;
            }
        }

        private void SetFontFamily()
        {
            if (_label.FontFamily == null) return;
            _builder.Append($"font-family: '{_label.FontFamily}'; ");
        }

        private void SetFontSize()
        {
            _builder.Append($"font-size: {_label.FontSize}px; ");
        }

        private void SetTextColor()
        {
            if (_label.TextColor.IsDefault) return;
            var color = _label.TextColor;
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
            _builder.Append($"color: {hex}; ");
        }

        
        private void SetHorizontalTextAlign()
        {
            if (_label.HorizontalTextAlignment == TextAlignment.Start) return;
            switch (_label.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    _builder.Append("text-align: center; ");
                    break;
                case TextAlignment.End:
                    _builder.Append("text-align: right; ");
                    break;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_label.Text))
                return string.Empty;

            _builder.Append("<div style=\"");
            SetFontAttributes();
            SetFontFamily();
            SetFontSize();
            SetTextColor();
            SetHorizontalTextAlign();
            
            _builder.Append($"\">{_text}</div>");
            var text = _builder.ToString();
            return text;
        }
    }
}
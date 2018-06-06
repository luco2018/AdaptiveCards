﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace AdaptiveCards.Rendering.Wpf
{
    public static class ImageExtensions
    {

        /// <summary>
        /// Renders the element to a bitmap
        /// </summary>
        public static MemoryStream RenderToImage(this FrameworkElement element, int width)
        {
            element.Measure(new Size(width, int.MaxValue));
            // Add 100 to the height to give it some buffer. This addressed some bugs with maxlines getting clipped
            element.Arrange(new Rect(new Size(width, element.DesiredSize.Height + 100)));
            element.UpdateLayout();

            var bitmapImage = new RenderTargetBitmap((int)width, (int)element.DesiredSize.Height, 96, 96,
                PixelFormats.Default);
            bitmapImage.Render(element);

            var encoder = new PngBitmapEncoder();
            var metadata = new BitmapMetadata("png");
            // TODO: Should we set the image metadata?
            //metadata.SetQuery("/tEXt/{str=Description}", JsonConvert.SerializeObject(OriginatingCard));
            var pngFrame = BitmapFrame.Create(bitmapImage, null, metadata, null);
            encoder.Frames.Add(pngFrame);

            var stream = new MemoryStream();
            encoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static async void SetSource(this Image image, Uri url, AdaptiveRenderContext context)
        {
            if (url == null)
                return;

            image.Source = await context.ResolveImageSource(url);

            if (image.Source == null)
                return;

            SetBinding(image);
        }

        public static void SetSource(this Image image, BitmapImage bi)
        {
            if (bi == null)
                return;

            image.Source = bi;

            SetBinding(image);
        }

        private static void SetBinding(Image image)
        {
            var binding = new Binding
            {
                RelativeSource = RelativeSource.Self,
                Path = new PropertyPath("Parent.ActualWidth"),
                Mode = BindingMode.OneWay,
                Converter = new StretchConverter(),
                ConverterParameter = image
            };

            image.SetBinding(Image.StretchProperty, binding);
        }

        public class StretchConverter : IValueConverter
        {

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var parentWidth = (double)value;
                var image = (Image)parameter;

                var imageWidth = ((BitmapImage) image.Source)?.PixelWidth;
                if (imageWidth >= parentWidth)
                {

                    return Stretch.Uniform;
                }
                else
                {
                    return Stretch.None;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public static async void SetBackgroundSource(this Grid grid, Uri uri, AdaptiveRenderContext context)
        {
            if (uri == null)
                return;

            BitmapImage bi = null;

            // Try to resolve the image URI
            if (uri.IsAbsoluteUri)
            {
                // If it's an absolute URI, simply set the source
                bi = await context.ResolveImageSource(uri);
            }
            else
            {
                // Otherwise, combine with image base URL and try again of specified
                if (!String.IsNullOrEmpty(context.Config.ImageBaseUrl))
                {
                    try
                    {
                        Uri baseUri = new Uri(context.Config.ImageBaseUrl);
                        Uri combinedUri = new Uri(baseUri, uri.ToString());
                        if (uri.IsAbsoluteUri)
                        {
                            bi = await context.ResolveImageSource(combinedUri);
                        }
                        else
                        {
                            // If it's still a relative URL, don't load and return
                            return;
                        }
                    }
                    catch (UriFormatException) { return; }
                }
            }

            if (bi != null)
            {
                grid.Background = new ImageBrush(bi)
                {
                    Stretch = Stretch.UniformToFill,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top
                };
            }
        }

        public static void SetImageProperties(this Image imageview, AdaptiveImage image, AdaptiveRenderContext context)
        {
            switch (image.Size)
            {
                case AdaptiveImageSize.Auto:
                    imageview.Stretch = Stretch.Uniform;
                    break;
                case AdaptiveImageSize.Stretch:
                    imageview.Stretch = Stretch.Uniform;
                    break;
                case AdaptiveImageSize.Small:
                    imageview.Width = context.Config.ImageSizes.Small;
                    imageview.Height = context.Config.ImageSizes.Small;
                    break;
                case AdaptiveImageSize.Medium:
                    imageview.Width = context.Config.ImageSizes.Medium;
                    imageview.Height = context.Config.ImageSizes.Medium;
                    break;
                case AdaptiveImageSize.Large:
                    imageview.Width = context.Config.ImageSizes.Large;
                    imageview.Height = context.Config.ImageSizes.Large;
                    break;
            }
        }
    }
}

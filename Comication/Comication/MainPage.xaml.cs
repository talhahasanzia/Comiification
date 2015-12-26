using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Lumia.Imaging;
using System.Threading.Tasks;
using Lumia.Imaging.Artistic;

using Lumia.Imaging.Transforms;
using Lumia.Imaging.Compositing;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Comication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Chroma Key Filter Variables
        private WriteableBitmap _bitmap = null;
        private DispatcherTimer _timer = null;
        private Color _color = new Color();
        private bool _initialized = false;
        private bool _rendering = false;
        private BitmapImage _source = null;
        private WriteableBitmapRenderer _renderer = null;
        private FilterEffect _effect = null;
        private IList<IFilter> _filters = null;
        private ChromaKeyFilter _chromaKeyFilter = null;
        private RotationFilter _rotationFilter = null;
        StorageFile filesample;
        Color BackgroundColor;
       private StorageFile SelectedImageFile;
       private StorageFile SelectedImageFile2;
        private 
            BitmapImage selectedImage;
        // Cartoon Filter Variables
        private FilterEffect _cartoonEffect = null;
        

        // The following  WriteableBitmap contains 
        // the filtered and thumbnail images.
        private WriteableBitmap _cartoonImageBitmap = null;

        

        public MainPage()
        {
           
           
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
           
            
           
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            
        }
        private async void Chroma_Click(object sender, RoutedEventArgs e) {



            await ApplyChromaFilterAsync();
        
        }
 
        
       
        
        
        
        
        private async Task<bool> ApplyCartoonFilterAsync(StorageFile file)
        {
            _cartoonImageBitmap = new WriteableBitmap(selectedImage.PixelHeight, selectedImage.PixelWidth);


            // Open a stream for the selected file. 
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

            string errorMessage = null;

            try
            {
                // Show the thumbnail of original image. 
                
                // Rewind the stream to start. 
                fileStream.Seek(0);

                // A cartoon effect is initialized with the selected image stream as source.  
                var imageSource = new RandomAccessStreamImageSource(fileStream);
                _cartoonEffect = new FilterEffect(imageSource);

                // Add the cartoon filter as the only filter for the effect. 
                var cartoonFilter = new CartoonFilter();
                _cartoonEffect.Filters = new[] { cartoonFilter };

                // Render the image to a WriteableBitmap. 
                var renderer = new WriteableBitmapRenderer(_cartoonEffect, _cartoonImageBitmap);
                _cartoonImageBitmap = await renderer.RenderAsync();
                _cartoonImageBitmap.Invalidate();

                // Set the rendered image as source for the cartoon image control. 
               FilteredView.Source = _cartoonImageBitmap;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
               
                return false;
            }

            return true;
        }

       private async Task<bool> ApplyChromaFilterAsync()
        {
            try
            {

               // await getFile();
               
                //FileNameText.Text = filesample.Name;


                BitmapImage bitmapImageBack = new BitmapImage();


                StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///colorWheel.png"));

                
                
               

                IRandomAccessStream fileStream = await SelectedImageFile.OpenAsync(FileAccessMode.Read);
                IRandomAccessStream backgroundStream = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
               bitmapImage.SetSource(backgroundStream);


                
                FilteredView.Source = bitmapImage;
               




                // Rewind the stream to start. 
                fileStream.Seek(0);
                backgroundStream.Seek(0);

                // Image 1 
                var imageSource = new RandomAccessStreamImageSource(fileStream);
                
                
                // Image 2
                var BackgroundImage = new RandomAccessStreamImageSource(backgroundStream);


                // Effect
                _effect = new FilterEffect(imageSource);




                // blend effect. 
                //var blendFilter = new BlendEffect(BackgroundImage, _effect, BlendFunction.Normal, 1.0);

                BackgroundColor = Color.FromArgb(255, 217, 204, 185);
                _effect.Filters = new IFilter[] { new ChromaKeyFilter(BackgroundColor, 0.2, 0.3, false) };
                // Create a target where the filtered image will be rendered to
                var target = new WriteableBitmap(selectedImage.PixelWidth, selectedImage.PixelHeight);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(_effect, target))
                {
                    await renderer.RenderAsync();

                    // Set the output image to Image control as a source
                    FilteredView.Source = target;


                }
            }
            catch (FileNotFoundException ex)
            {
                ExceptionText.Text = ex.Message;
            }
            catch (Exception e)
            {
                ExceptionText.Text=e.Message;
            }

            return true;
        
        
        }


       
        
        private void FilteredView_Tapped(object sender, TappedRoutedEventArgs e)
       {
           var Point = e.GetPosition(FilteredView);
          
           var bitmap = new WriteableBitmap((int)FilteredView.ActualWidth, (int)FilteredView.ActualHeight);

         


          
            // ERROR HERE
               //var picked = bitmap.Pixels[((int)Point.Y) * bitmap.PixelWidth + ((int)Point.X)];

               //_color = new Color
               //{
               //    A = 0xFF,
               //    R = (byte)((picked & 0x00FF0000) >> 16),
               //    G = (byte)((picked & 0x0000FF00) >> 8),
               //    B = (byte)(picked & 0x000000FF)
               //};
          
          
           
       }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ComicationView));
        }

        
    }
}

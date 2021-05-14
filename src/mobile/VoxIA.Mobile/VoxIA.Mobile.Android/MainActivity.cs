using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using VoxIA.Core.Media;
using Xamarin.Forms;

namespace VoxIA.Mobile.Droid
{
    [Activity(Label = "VoxIA.Mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            var stream = Assets.Open("config.client");
            var properties = new Properties();
            properties.Load(stream);
            Java.Util.IEnumeration props = properties.PropertyNames();
            var iceProperties = new Dictionary<string, string>();
            while (props.HasMoreElements)
            {
                var name = (string)props.NextElement();
                iceProperties.Add(name, properties.GetProperty(name));
            }

            //Stream certStream;
            //X509Certificate2Collection certsCA = new X509Certificate2Collection();
            //certStream = Resources.OpenRawResource(Resource.Raw.cacert);
            //using (var ms = new MemoryStream())
            //{
            //    certStream.CopyTo(ms);
            //    certsCA.Import(ms.ToArray(), "password", X509KeyStorageFlags.DefaultKeySet);
            //}

            //X509Certificate2Collection certs = new X509Certificate2Collection();
            //certStream = Resources.OpenRawResource(Resource.Raw.client1);
            //using (var ms = new MemoryStream())
            //{
            //    certStream.CopyTo(ms);
            //    certs.Import(ms.ToArray(), "password", X509KeyStorageFlags.DefaultKeySet);
            //}

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(iceProperties));
            //DependencyService.Register<IMediaPlayer, AndroidMediaPlayer>();
            DependencyService.Register<IMetadataRetriever, Id3MetadataRetriever>();
            DependencyService.Register<IMediaRecorder, AndroidMediaRecorder>();
            
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 1);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
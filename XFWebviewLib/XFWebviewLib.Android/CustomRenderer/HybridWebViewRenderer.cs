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
using Xamarin.Forms;
using XFWebviewLib.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using XFWebviewLib.CustomRenderer;
using Android.Webkit;
using Java.Interop;
using Object = Java.Lang.Object;
using WebView = Android.Webkit.WebView;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace XFWebviewLib.Droid.CustomRenderer
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        /// <summary>
        /// Allows one to override the Webview Client class without a custom renderer.
        /// </summary>
        public static Func<HybridWebViewRenderer, Client> GetWebViewClientDelegate;

        /// <summary>
        /// Allows one to override the Chrome Client class without a custom renderer.
        /// </summary>
        public static Func<HybridWebViewRenderer, ChromeClient> GetWebChromeClientDelegate;

        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

        public HybridWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null && e.NewElement != null)
            {
                var webView = new Android.Webkit.WebView(Control.Context);

                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;

                //Turn off hardware rendering
                webView.SetLayerType(e.NewElement.AndroidHardwareRendering ? LayerType.Hardware : LayerType.Software, null);

                //Set the background color to transparent to fix an issue where the
                //the picture would fail to draw
                webView.SetBackgroundColor(Color.Transparent.ToAndroid());

                webView.SetWebViewClient(this.GetWebViewClient());
                webView.SetWebChromeClient(this.GetWebChromeClient());

                webView.AddJavascriptInterface(new JSBridge(this), "jsBridge");

                this.SetNativeControl(webView);

                webView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            }

            this.Unbind(e.OldElement);

            this.Bind();
        }

        void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        private void Bind()
        {
            if (Element != null)
            {
                if (this.Element.Uri != null)
                {
                    this.Load(this.Element.Uri);
                }
                else
                {
                    LoadSource();
                }

                // There should only be one renderer and thus only one event handler registered.
                // Otherwise, when Xamarin creates a new renderer, the old one stays attached
                // and crashes when called!
                this.Element.JavaScriptLoadRequested = OnInjectRequest;
                this.Element.LoadFromContentRequested = LoadFromContent;
                this.Element.LoadContentRequested = LoadContent;
            }
        }

        private void Unbind(HybridWebView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.JavaScriptLoadRequested -= OnInjectRequest;
                oldElement.LoadFromContentRequested -= LoadFromContent;
                oldElement.LoadContentRequested -= LoadContent;
                oldElement.PropertyChanged -= this.OnElementPropertyChanged;
            }
        }

        private void OnInjectRequest(object sender, string script)
        {
            this.Inject(script);
        }

        private void LoadSource()
        {
            var htmlSource = this.Element.Source as HtmlWebViewSource;
            if (htmlSource != null)
            {
                this.LoadFromString(htmlSource.Html);
                return;
            }

            var webViewSource = this.Element.Source as UrlWebViewSource;

            if (webViewSource != null)
            {
                this.Load(new Uri(webViewSource.Url));
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.Element != null)
            {
                if (this.Control != null)
                {
                    this.Control.StopLoading();
                }

                Unbind(this.Element);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets <see cref="Client"/> for the web view.
        /// </summary>
        /// <returns><see cref="Client"/></returns>
        protected virtual Client GetWebViewClient()
        {
            var d = GetWebViewClientDelegate;

            return d != null ? d(this) : new Client(this);
        }

        /// <summary>
        /// Gets <see cref="ChromeClient"/> for the web view.
        /// </summary>
        /// <returns><see cref="ChromeClient"/></returns>
        protected virtual ChromeClient GetWebChromeClient()
        {
            var d = GetWebChromeClientDelegate;

            return d != null ? d(this) : new ChromeClient();
        }

        private void OnPageFinished()
        {
            if (this.Element == null) return;
            //this.Inject(NativeFunction);
            //this.Inject(GetFuncScript());
            this.Element.OnLoadFinished(this, EventArgs.Empty);
        }

        /// <summary>
        /// Injects the specified script.
        /// </summary>
        /// <param name="script">The script.</param>
        void Inject(string script)
        {
            if (Control != null)
            {
                this.Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        /// <summary>
        /// Loads the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
         void Load(Uri uri)
        {
            if (uri != null && Control != null)
            {
                this.Control.LoadUrl(uri.AbsoluteUri);
            }
        }

        /// <summary>
        /// Loads from content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="contentFullName">Full name of the content.</param>
         void LoadFromContent(object sender, HybridWebView.LoadContentEventArgs contentArgs)
        {
            var baseUri = new Uri(contentArgs.BaseUri ?? "file:///android_asset/");
            this.Element.Uri = new Uri(baseUri, contentArgs.Content);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="content">Full name of the content.</param>
         void LoadContent(object sender, HybridWebView.LoadContentEventArgs contentArgs)
        {
            if (Control != null)
            {
                var baseUri = contentArgs.BaseUri ?? "file:///android_asset/";
                this.Control.LoadDataWithBaseURL(baseUri, contentArgs.Content, "text/html", "UTF-8", null);
            }
        }

        /// <summary>
        /// Loads from string.
        /// </summary>
        /// <param name="html">The HTML.</param>
         void LoadFromString(string html)
        {
            if (Control != null)
            {
                this.Control.LoadData(html, "text/html", "UTF-8");
            }
        }
        /// <summary>
        /// Class ChromeClient.
        /// </summary>
        public class ChromeClient : WebChromeClient
        {
            /// <summary>
            /// Overrides the geolocation prompt and accepts the permission.
            /// </summary>
            /// <param name="origin">Origin of the prompt.</param>
            /// <param name="callback">Permission callback.</param>
            /// <remarks>Always grant permission since the app itself requires location
            /// permission and the user has therefore already granted it.</remarks>
            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
            {
                callback.Invoke(origin, true, false);
            }
        }

        /// <summary>
        /// Class Client.
        /// </summary>
        public class Client : WebViewClient
        {
            /// <summary>
            /// The web hybrid
            /// </summary>
            protected readonly WeakReference<HybridWebViewRenderer> WebHybrid;

            /// <summary>
            /// Initializes a new instance of the <see cref="Client"/> class.
            /// </summary>
            /// <param name="webHybrid">The web hybrid.</param>
            public Client(HybridWebViewRenderer webHybrid)
            {
                this.WebHybrid = new WeakReference<HybridWebViewRenderer>(webHybrid);
            }

            /// <summary>
            /// Notify the host application that a page has finished loading.
            /// </summary>
            /// <param name="view">The WebView that is initiating the callback.</param>
            /// <param name="url">The url of the page.</param>
            /// <since version="Added in API level 1" />
            /// <remarks><para tool="javadoc-to-mdoc">Notify the host application that a page has finished loading. This method
            /// is called only for main frame. When onPageFinished() is called, the
            /// rendering picture may not be updated yet. To get the notification for the
            /// new Picture, use <c><see cref="M:Android.Webkit.WebView.IPictureListener.OnNewPicture(Android.Webkit.WebView, Android.Graphics.Picture)" /></c>.</para>
            /// <para tool="javadoc-to-mdoc">
            ///   <format type="text/html">
            ///     <a href="http://developer.android.com/reference/android/webkit/WebViewClient.html#onPageFinished(android.webkit.WebView, java.lang.String)" target="_blank">[Android Documentation]</a>
            ///   </format>
            /// </para></remarks>
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                HybridWebViewRenderer hybrid;
                if (this.WebHybrid != null && this.WebHybrid.TryGetTarget(out hybrid))
                {
                    hybrid.OnPageFinished();
                }
            }
        }

    }


}
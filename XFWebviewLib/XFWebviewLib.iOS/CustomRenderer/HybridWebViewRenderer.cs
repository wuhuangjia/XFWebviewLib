using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFWebviewLib.CustomRenderer;
using XFWebviewLib.iOS.CustomRenderer;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace XFWebviewLib.iOS.CustomRenderer
{
    class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController _userController;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                _userController = new WKUserContentController();
                var config = new WKWebViewConfiguration()
                {
                    UserContentController = _userController
                };

                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);

                _userController.AddUserScript(script);

                _userController.AddScriptMessageHandler(this, "invokeAction");

                var webView = new WKWebView(Frame, config) { WeakNavigationDelegate = this };

                SetNativeControl(webView);

            }

            Unbind(e.OldElement);
            Bind();
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

        /// <summary>
        /// Injects the specified script.
        /// </summary>
        /// <param name="script">The script.</param>
        void Inject(string script)
        {
            InvokeOnMainThread(() => Control.EvaluateJavaScript(new NSString(script), (r, e) =>
            {
                if (e != null) Debug.WriteLine(e);
            }));
        }


        void Load(Uri uri)
        {
            if (uri != null)
            {
                Control.LoadRequest(new NSUrlRequest(new NSUrl(uri.AbsoluteUri)));
            }
        }

        void LoadFromContent(object sender, HybridWebView.LoadContentEventArgs contentArgs)
        {
            var baseUri = contentArgs.BaseUri ?? GetTempDirectory();
            Element.Uri = new Uri(baseUri + "/" + contentArgs.Content);
            //Element.Uri = new Uri(NSBundle.MainBundle.BundlePath + "/" + contentFullName);
        }

        void LoadContent(object sender, HybridWebView.LoadContentEventArgs contentArgs)
        {
            var baseUri = contentArgs.BaseUri ?? GetTempDirectory();
            Control.LoadHtmlString(new NSString(contentArgs.Content), new NSUrl(baseUri, true));
        }

        void LoadFromString(string html)
        {
            LoadContent(null, new HybridWebView.LoadContentEventArgs(html, null));
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

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            Element.InvokeAction(message.Body.ToString());
        }

        private static string GetTempDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("Documents", "tmp");
        }
    }
}

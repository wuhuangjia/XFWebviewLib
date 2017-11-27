using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XFWebviewLib.Helper;

namespace XFWebviewLib.CustomRenderer
{
    public class HybridWebView : View
    {
        Action<string> action;

        /// <summary>
        /// The uri property.
        /// </summary>
        public static readonly BindableProperty UriProperty = BindableProperty.Create("Uri", typeof(Uri), typeof(HybridWebView), default(Uri));

        /// <summary>
        /// The source property.
        /// </summary>
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(WebViewSource), typeof(HybridWebView), default(WebViewSource));

        /// <summary>
        /// Boolean to indicate cleanup has been called.
        /// </summary>
        public static readonly BindableProperty CleanupProperty = BindableProperty.Create("CleanupCalled", typeof(bool), typeof(HybridWebView), false);

        /// <summary>
        /// Enable/Disable android hardware webpage rendering.
        /// </summary>
        public static readonly BindableProperty AndroidHardwareRenderingProperty = BindableProperty.Create("AndroidHardwareRendering", typeof(bool), typeof(HybridWebView), false);

        /// <summary>
        /// Enable/Disable additional android touch callback.
        /// </summary>
        public static readonly BindableProperty AndroidAdditionalTouchCallbackProperty = BindableProperty.Create("AndroidAdditionalTouchCallback", typeof(bool), typeof(HybridWebView), true);

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri
        {
            get { return (Uri)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public WebViewSource Source
        {
            get { return (WebViewSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// The java script load requested
        /// </summary>
        public EventHandler<string> JavaScriptLoadRequested;

        /// <summary>
        /// The load from content requested
        /// </summary>
        public EventHandler<LoadContentEventArgs> LoadFromContentRequested;

        /// <summary>
        /// The load content requested
        /// </summary>
        public EventHandler<LoadContentEventArgs> LoadContentRequested;

        /// <summary>
        /// The load finished
        /// </summary>
        public EventHandler LoadFinished;

        /// <summary>
        /// The navigating
        /// </summary>
        public EventHandler<EventArgs<Uri>> Navigating;

        /// <summary>
        /// The inject lock.
        /// </summary>
        private readonly object injectLock = new object();

        /// <summary>
        /// Gets or sets the cleanup called flag.
        /// </summary>
        public bool CleanupCalled
        {
            get { return (bool)GetValue(CleanupProperty); }
            set { SetValue(CleanupProperty, value); }
        }

        /// <summary>
        /// Gets or sets android hardware rendering flag.
        /// </summary>
        public bool AndroidHardwareRendering
        {
            get { return (bool)GetValue(AndroidHardwareRenderingProperty); }
            set { SetValue(AndroidHardwareRenderingProperty, value); }
        }

        /// <summary>
        /// Gets or sets android additional touch callback. 
        /// </summary>
        public bool AndroidAdditionalTouchCallback
        {
            get { return (bool)GetValue(AndroidAdditionalTouchCallbackProperty); }
            set { SetValue(AndroidAdditionalTouchCallbackProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="contentFullName">Full name of the content.</param>
        /// <param name="baseUri">Optional base Uri to use for resources.</param>
        public void LoadFromContent(string contentFullName, string baseUri = null)
        {
            this.LoadFromContentRequested?.Invoke(this, new LoadContentEventArgs(contentFullName, baseUri));
        }

        /// <summary>
        /// Loads the content from string content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="baseUri">Optional base Uri to use for resources.</param>
        public void LoadContent(string content, string baseUri = null)
        {
            this.LoadContentRequested?.Invoke(this, new LoadContentEventArgs(content, baseUri));
        }

        /// <summary>
        /// Injects the java script.
        /// </summary>
        /// <param name="script">The script.</param>
        public void InjectJavaScript(string script)
        {
            lock (this.injectLock)
            {
                this.JavaScriptLoadRequested?.Invoke(this, script);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:LoadFinished" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void OnLoadFinished(object sender, EventArgs e)
        {
            this.LoadFinished?.Invoke(this, e);
        }

        /// <summary>
        /// Called when [navigating].
        /// </summary>
        /// <param name="uri">The URI.</param>
        public void OnNavigating(Uri uri)
        {
            this.Navigating?.Invoke(this, new EventArgs<Uri>(uri));
        }

        public class LoadContentEventArgs : EventArgs
        {
            public LoadContentEventArgs(string content, string baseUri)
            {
                this.Content = content;
                this.BaseUri = baseUri;
            }

            public string Content { get; private set; }
            public string BaseUri { get; private set; }
        }

    }
}

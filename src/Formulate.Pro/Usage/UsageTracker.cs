namespace Formulate.Pro.Usage
{

    // Namespaces.
    using System.Net;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Operations related to tracking Formulate Pro usage.
    /// </summary>
    internal class UsageTracker
    {

        #region Properties

        /// <summary>
        /// Lock object to avoid cross-threading issues when tracking usage.
        /// </summary>
        private static object TrackingLock { get; } = new object();

        /// <summary>
        /// Was the designed email functionality already tracked?
        /// </summary>
        private static bool WasDesignedEmailTracked { get; set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// Tracks that the designed email feature was used.
        /// </summary>
        public static void TrackDesignedEmail()
        {

            // Only track this feature once.
            if (WasDesignedEmailTracked)
            {
                return;
            }
            lock (TrackingLock)
            {
                if (WasDesignedEmailTracked)
                {
                    return;
                }
                WasDesignedEmailTracked = true;
            }

            // Variables.
            var domain = default(string);
            var referrer = default(string);

            // Get the domain and referrer from the main thread.
            try
            {
                var request = HttpContext.Current.Request;
                domain = request.Url.Authority;
                referrer = request.UrlReferrer?.ToString();
            }
            catch
            {
                return;
            }

            // Track the usage on a background thread to avoid slowing down the main thread.
            new Thread(() =>
            {
                try
                {

                    // Construct the URL.
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["domain"] = domain;
                    query["referrer"] = referrer;
                    query["feature"] = "Designed Email";
                    var url = $"http://pro-usage.formulate.rocks/log-usage?" + query.ToString();

                    // Send a request to log usage of the designed email feature.
                    using (var client = new WebClient())
                    {
                        client.DownloadString(url);
                    }

                }
                catch
                {
                }
            }).Start();

        }

        #endregion

    }

}
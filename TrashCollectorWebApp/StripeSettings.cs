using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashCollectorWebApp
{
    public class StripeSettings
    {
        private string secretKey = ApiKey.STRIPE_SECRET;
        private string publishableKey = ApiKey.STRIPE_PUBLISHABLE;
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
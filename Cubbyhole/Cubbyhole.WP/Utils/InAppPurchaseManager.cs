using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubbyhole.Models;
using Windows.ApplicationModel.Store;
using System.Net;
using Cubbyhole.WP.Resources;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cubbyhole.WP.Utils
{
    public delegate void PlanPurchasedChangedEventHandler(object sender, EventArgs e, Plan plan);
    public class InAppPurchaseManager
    {
        public event PlanPurchasedChangedEventHandler PlanPurchased;
        private InformerManagerLocator _informerManagerLocator { get; set; }

        public InAppPurchaseManager()
        {
            _informerManagerLocator = new InformerManagerLocator();
        }

        public async void BuyProductByName(Plan plan)
        {

            ListingInformation allProducts = await CurrentApp.LoadListingInformationAsync();
            ProductListing productListing = null;
            if (!allProducts.ProductListings.TryGetValue(plan.Name, out productListing))
            {
                _informerManagerLocator.InformerManager.AddMessage("Error",
                                                                    "Can't find this plan in market, sorry for this inconvenience");
                PlanPurchased(this, new EventArgs(), null);
            }
            else
            {
                try
                {
                    /********************************/
                    // DON'T DO THIS WITH TRUE APP
                    var licensetest = CurrentApp.LicenseInformation.ProductLicenses[plan.Name];
                    if (licensetest.IsConsumable && licensetest.IsActive)
                    {
                        CurrentApp.ReportProductFulfillment(licensetest.ProductId);
                    }
                    /*******************************/

                    string result = await CurrentApp.RequestProductPurchaseAsync(productListing.ProductId, false);
                    var license = CurrentApp.LicenseInformation.ProductLicenses[plan.Name];
                    if (license.IsConsumable && license.IsActive)
                    {
                        CurrentApp.ReportProductFulfillment(license.ProductId);
                            
                        //Pour envoyer un avis de reception chiffré
                        /*string receiptXml = await CurrentApp.GetProductReceiptAsync(license.ProductId);
                        await SendReceipt(receiptXml);*/
                    }

                    PlanPurchased(this, new EventArgs(), plan);
                }
                catch (Exception)
                {
                    if (PlanPurchased != null)
                    {
                        PlanPurchased(this, new EventArgs(), null);
                    }
                }
            }
        }

        /*async Task SendReceipt(string receiptXml)
        {
            Uri baseAddress = new Uri(AppResources.BaseAddress);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress + "payment_notifications");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("receiptXml", receiptXml);
            HttpContent httpContent = new StringContent(receiptXml, Encoding.UTF8, "application/xml");
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();
        }*/
    }
}

using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using VirtoCommerce.Domain.Payment.Model;
//using Newtonsoft.Json;
using System.Security.Permissions;
using System.Security.Claims;
using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;
using System.Collections;
using VirtoCommerce.Platform.Data.Security.Identity;
using System.Linq;

namespace TLB.Payments.Managers
{
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]

    public class TLBpaymentSystem : VirtoCommerce.Domain.Payment.Model.PaymentMethod
    {

        private const string _TLBID = "TLB.Payments.ID";
        private const string _TLBPoints = "TLB.Payments.Points";


        public TLBpaymentSystem() : base("TLB.Payments")
        {

        }
        public string MemberId
        {
            get
            {
                var retVal = GetSetting(_TLBID);
                return retVal;
            }
        }

        public string Points
        {

            get
            {
                var retVal = GetSetting(_TLBPoints);
                return retVal;
            }
        }


        public override PaymentMethodType PaymentMethodType
        {
            get { return VirtoCommerce.Domain.Payment.Model.PaymentMethodType.Unknown; }
        }

        public override PaymentMethodGroupType PaymentMethodGroupType
        {
            get { return VirtoCommerce.Domain.Payment.Model.PaymentMethodGroupType.Manual; }
        }


        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            ProcessPaymentResult retVal = new ProcessPaymentResult();

            try
            {
                string Email = context.Order.Addresses.FirstOrDefault().Email;
                if (context.Order.Items.Count >= 1)
                {
                    context.Payment.PaymentStatus = PaymentStatus.Authorized;
                    context.Payment.OuterId = context.Payment.Number;
                    context.Payment.CapturedDate = DateTime.UtcNow;
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://tlsnewmyweb.azurewebsites.net/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/TLSMobileAuthentication/getallpoints?Email=" + Email + "&point=" + Convert.ToInt32(context.Order.Sum)).Result;
                    string obj = response.Content.ReadAsStringAsync().Result;
                    retVal.IsSuccess = true;

                }

            }
            catch (Exception ex)
            {

                retVal.Error = ex.Message;
            }

            return retVal;
        }


        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {

            context.Payment.PaymentStatus = PaymentStatus.Authorized;
            context.Payment.OuterId = context.Payment.Number;
            context.Payment.CapturedDate = DateTime.UtcNow;

            return new PostProcessPaymentResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Authorized };
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {

            context.Payment.IsApproved = false;
            context.Payment.PaymentStatus = PaymentStatus.Voided;
            context.Payment.VoidedDate = DateTime.UtcNow;
            context.Payment.IsCancelled = true;
            context.Payment.CancelledDate = DateTime.UtcNow;
            return new VoidProcessPaymentResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Voided };
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {



            throw new NotImplementedException(); ;

        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {


            return new RefundProcessPaymentResult { ErrorMessage = "Not implemented yet" };
        }

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
        {


            throw new NotImplementedException();
        }


    }

}

using System;
using Microsoft.Practices.Unity;
using TLB.Payments.Managers;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;


namespace TLB.Payments
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;

        }


        #region IModule Members

        public override void Initialize()
        {
            var settings = _container.Resolve<ISettingsManager>().GetModuleSettings("TLB.Payments");

            Func<TLBpaymentSystem> TlbPaymentMethodFactory = () => new TLBpaymentSystem
            {
                Name = "TLB payments",
                Description = "TLB payment integration",
                LogoUrl = "",
                Settings = settings
            };

            _container.Resolve<IPaymentMethodsService>().RegisterPaymentMethod(TlbPaymentMethodFactory);




        }

        #endregion

    }
}
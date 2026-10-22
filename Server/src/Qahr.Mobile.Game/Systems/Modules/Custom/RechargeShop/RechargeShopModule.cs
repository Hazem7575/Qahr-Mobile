// RechargeShop - migrated into the module system.
// The feature logic stays in RechargeShop.cs (same folder, namespace Qahr.Mobile.Database);
// this module owns its lifecycle: it loads the recharge-shop data at startup and persists it on
// every world save. Previously these two calls were hard-wired into Database\Server.cs.
namespace Qahr.Mobile.Modules.Custom
{
    public sealed class RechargeShopModule : ModuleBase
    {
        public override string Name { get { return "RechargeShop"; } }
        public override string Version { get { return "1.0"; } }

        public override void OnLoad()
        {
            Qahr.Mobile.Database.RechargeShop.Load();
            Log("recharge-shop data loaded.");
        }

        public override void OnSave()
        {
            Qahr.Mobile.Database.RechargeShop.Save();
        }

        // رجّع تقدّم الشحن المحفوظ للاعب عند الدخول
        public override void OnLogin(Client.GameClient client)
        {
            Qahr.Mobile.Database.RechargeShop.RestoreRecharge(client);
        }
    }
}

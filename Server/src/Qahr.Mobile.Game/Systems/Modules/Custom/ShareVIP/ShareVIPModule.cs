// ShareVIP - first real custom feature migrated into the module system (proof of concept).
// The feature logic stays in ShareVIP.cs (same folder); this module just owns its lifecycle:
// it loads the shared-VIP list at startup and persists it on every world save. Previously these
// two calls were hard-wired into Database\Server.cs - now they live with the feature.
namespace Qahr.Mobile.Modules.Custom
{
    public sealed class ShareVIPModule : ModuleBase
    {
        public override string Name { get { return "ShareVIP"; } }
        public override string Version { get { return "1.0"; } }
        // Always on (it is a core-ish convenience feature, not a timed event). Give it a Schedule to make
        // it a timed event instead (see Modules\Custom\README.txt).

        public override void OnLoad()
        {
            Qahr.Mobile.Database.ShareVIP.Load();
            Log("shared-VIP list loaded.");
        }

        public override void OnSave()
        {
            Qahr.Mobile.Database.ShareVIP.Save();
        }
    }
}

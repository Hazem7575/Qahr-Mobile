// VoteSystem - migrated into the module system.
// The feature logic stays in VoteSystem.cs (same folder, namespace Qahr.Mobile.Database);
// this module just owns its lifecycle: it loads the vote data at startup and persists it on
// every world save. Previously the load call was hard-wired into Database\Server.cs.
namespace Qahr.Mobile.Modules.Custom
{
    public sealed class VoteSystemModule : ModuleBase
    {
        public override string Name { get { return "VoteSystem"; } }
        public override string Version { get { return "1.0"; } }

        public override void OnLoad()
        {
            Qahr.Mobile.Database.VoteSystem.Load();
            Log("vote data loaded.");
        }

        public override void OnSave()
        {
            Qahr.Mobile.Database.VoteSystem.Save();
        }
    }
}

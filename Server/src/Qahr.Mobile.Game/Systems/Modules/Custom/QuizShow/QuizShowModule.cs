// QuizShow - migrated into the module system.
// The feature logic stays in QuizShow.cs (same folder, namespace Qahr.Mobile.Database);
// this module owns its lifecycle: it loads the quiz data at startup. Previously the load call
// was hard-wired into Database\Server.cs. (QuizShow exposes no Save, so no OnSave here.)
namespace Qahr.Mobile.Modules.Custom
{
    public sealed class QuizShowModule : ModuleBase
    {
        public override string Name { get { return "QuizShow"; } }
        public override string Version { get { return "1.0"; } }

        public override void OnLoad()
        {
            Qahr.Mobile.Database.QuizShow.Load();
            Log("quiz data loaded.");
        }
    }
}

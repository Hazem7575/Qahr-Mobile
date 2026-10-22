// ============================================================================
//  QahrMobile: things a brand-new mobile player should not be bothered with.
//  QuietEvents (MobileEvents.Divert): below EventDialogLevel the event invitations ("Dark Flag Base Just Started Do
//  You Want To Join Now !", boss spawns ...) come as a chat line only, never as a dialog that covers the screen. The
//  events themselves need a much higher level, and a level-2 player died with the dialog open in the Pheasant field.
//  A player who chose "@events dialog" himself still gets the dialogs.
// ============================================================================
namespace Qahr.Mobile.Phone
{
    public static class MobileNovice
    {
        public const int EventDialogLevel = 40;

        public static bool QuietEvents(Client.GameClient c)
        {
            return c != null && c.Player != null && c.Player.Level < EventDialogLevel && c.Player.Reborn == 0;
        }
    }
}

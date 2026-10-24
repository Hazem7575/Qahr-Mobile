using System;
namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: monster type tweaks read from Data\Monsters\*.ini (called by Server.ParseMonsterFamily, so
    /// `admin.py reloadmonsters` applies them live).
    /// attack_user=0 makes the monster passive: it never picks a target by itself (view range 0), like the
    /// beginner monsters around Twin City on the official client (Pheasant, Turtledove, Robin).
    /// </summary>
    public static class MobileMonsterTune
    {
        public static byte ViewRange(WindowsAPI.IniFile ini, byte current)
        {
            try
            {
                int au = ini.ReadInt32("cq_monstertype", "attack_user", 3);
                // passive types keep their view range so they can fight back; MobilePassiveAggro decides who they attack
                MobilePassiveAggro.MarkType(ini.ReadUInt32("cq_monstertype", "id", 0), au == 0);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            return current;
        }
    }
}

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: the one place that answers "is this player on the mobile client?".
    /// GameClient.IsMobile is set by MobileProtocol.Login (CMsgConnect 10124); PC clients never set it.
    /// </summary>
    public static class MobileClient
    {
        public static bool Is(Client.GameClient client)
        {
            return client != null && client.IsMobile;
        }
    }
}

namespace Mirage.Common.Network {
    /// <summary>
    /// Packet Type ID Enumeration
    /// </summary>
    public enum PacketType : short {
        Connection,
        Login,
        Register,

        PacketTypeCount /* Number of Packet Types */
    }
}
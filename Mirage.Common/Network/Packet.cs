using Lidgren.Network;

namespace Mirage.Common.Network {
    /// <summary>
    /// Class structure for packets.
    /// </summary>
    /// <remarks>
    /// The Packet class is completely generic and needs no subclasses, and is sealed.
    /// </remarks>
    public sealed class Packet {
        /// <summary>
        /// The type id of this packet
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The Lidgren message for this packet
        /// </summary>
        public NetIncomingMessage Message { get; set; }

        /// <summary>
        /// Flag for whether the packet has been handled or not
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Constructs a new Packet with the provided id and message.
        /// </summary>
        /// <param name="id">Packet type id</param>
        /// <param name="message">Packet message</param>
        public Packet(int id, NetIncomingMessage message) {
            ID = id;
            Message = message;
        }
    }
}
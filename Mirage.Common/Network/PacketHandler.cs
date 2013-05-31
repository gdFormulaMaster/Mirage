using System;
using Lidgren.Network;

namespace Mirage.Common.Network {
    /// <summary>
    /// Abstract Packet Handler class definition
    /// </summary>
    public abstract class PacketHandler {
        /// <summary>
        /// Packet Type ID for the PacketHandler
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Network peer for the PacketHandler
        /// </summary>
        public NetPeer Peer { get; set; }

        /// <summary>
        /// Constructs a PacketHandler with the specified packet type id and peer
        /// </summary>
        /// <param name="id">Type ID of the Packet Handler</param>
        /// <param name="peer">The <see cref="NetPeer"/> to use for outgoing messages</param>
        public PacketHandler(int id, NetPeer peer) {
            if (peer == null) {
                throw new ArgumentNullException();
            }

            ID = id;
            Peer = peer;
        }

        /// <summary>
        /// Handles the provided <see cref="Packet"/> and returns whether it was handled.
        /// </summary>
        /// <param name="packet">The packet to handle</param>
        /// <returns>Whether the packet was handled</returns>
        public virtual bool HandlePacket(Packet packet) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the provided <see cref="NetIncomingMessage"/> and returns whether it was handled.
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>Whether the message was handled</returns>
        public virtual bool HandleMessage(NetIncomingMessage message) {
            throw new NotImplementedException();
        }
    }
}
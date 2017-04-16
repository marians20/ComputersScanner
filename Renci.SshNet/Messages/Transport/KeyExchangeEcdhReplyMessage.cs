namespace Renci.SshNet.Messages.Transport
{
    /// <summary>
    /// Represents SSH_MSG_KEXECDH_REPLY message.
    /// </summary>
    [Message("SSH_MSG_KEXECDH_REPLY", 31)]
    public class KeyExchangeEcdhReplyMessage : Message
    {
        /// <summary>
        /// Gets a string encoding an X.509v3 certificate containing the server's ECDSA public host key
        /// </summary>
        /// <value>The host key.</value>
        public byte[] Ks { get; private set; }

        /// <summary>
        /// Gets the server's ephemeral contribution to the ECDH exchange, encoded as an octet string.
        /// </summary>
        public byte[] Qs { get; private set; }

        /// <summary>
        /// Gets an octet string containing the server's signature of the newly established exchange hash value.
        /// </summary>
        /// <value>The signature.</value>
        public byte[] Signature { get; private set; }

        /// <summary>
        /// Gets the size of the message in bytes.
        /// </summary>
        /// <value>
        /// The size of the messages in bytes.
        /// </value>
        protected override int BufferCapacity
        {
            get
            {
                var capacity = base.BufferCapacity;
                capacity += 4; // KS length
                capacity += Ks.Length; // KS
                capacity += 4; // QS length
                capacity += Qs.Length; // QS
                capacity += 4; // Signature length
                capacity += Signature.Length; // Signature
                return capacity;
            }
        }

        /// <summary>
        /// Called when type specific data need to be loaded.
        /// </summary>
        protected override void LoadData()
        {
            ResetReader();
            Ks = ReadBinary();
            Qs = ReadBinary();
            Signature = ReadBinary();
        }

        /// <summary>
        /// Called when type specific data need to be saved.
        /// </summary>
        protected override void SaveData()
        {
            WriteBinaryString(Ks);
            WriteBinaryString(Qs);
            WriteBinaryString(Signature);
        }
    }
}

using System;
using System.Text;
using Renci.SshNet.Messages.Transport;
using Renci.SshNet.Common;

namespace Renci.SshNet.Security
{
    /// <summary>
    /// Represents base class for Diffie Hellman key exchange algorithm
    /// </summary>
    public abstract class KeyExchangeDiffieHellman : KeyExchange
    {
        /// <summary>
        /// Specifies key exchange group number.
        /// </summary>
        protected BigInteger Group;

        /// <summary>
        /// Specifies key exchange prime number.
        /// </summary>
        protected BigInteger Prime;

        /// <summary>
        /// Specifies client payload
        /// </summary>
        protected byte[] ClientPayload;

        /// <summary>
        /// Specifies server payload
        /// </summary>
        protected byte[] ServerPayload;

        /// <summary>
        /// Specifies client exchange number.
        /// </summary>
        protected BigInteger ClientExchangeValue;

        /// <summary>
        /// Specifies server exchange number.
        /// </summary>
        protected BigInteger ServerExchangeValue;

        /// <summary>
        /// Specifies random generated number.
        /// </summary>
        protected BigInteger RandomValue;

        /// <summary>
        /// Specifies host key data.
        /// </summary>
        protected byte[] HostKey;

        /// <summary>
        /// Specifies signature data.
        /// </summary>
        protected byte[] Signature;

        /// <summary>
        /// Validates the exchange hash.
        /// </summary>
        /// <returns>
        /// true if exchange hash is valid; otherwise false.
        /// </returns>
        protected override bool ValidateExchangeHash()
        {
            var exchangeHash = CalculateHash();

            var length = (uint)(HostKey[0] << 24 | HostKey[1] << 16 | HostKey[2] << 8 | HostKey[3]);

            var algorithmName = Encoding.UTF8.GetString(HostKey, 4, (int)length);

            var key = Session.ConnectionInfo.HostKeyAlgorithms[algorithmName](HostKey);

            Session.ConnectionInfo.CurrentHostKeyAlgorithm = algorithmName;

            if (CanTrustHostKey(key))
            {

                return key.VerifySignature(exchangeHash, Signature);
            }
            return false;
        }

        /// <summary>
        /// Starts key exchange algorithm
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="message">Key exchange init message.</param>
        public override void Start(Session session, KeyExchangeInitMessage message)
        {
            base.Start(session, message);

            ServerPayload = message.GetBytes();
            ClientPayload = Session.ClientInitMessage.GetBytes();
        }

        /// <summary>
        /// Populates the client exchange value.
        /// </summary>
        protected void PopulateClientExchangeValue()
        {
            if (Group.IsZero)
                throw new ArgumentNullException("_group");

            if (Prime.IsZero)
                throw new ArgumentNullException("_prime");

            var bitLength = Prime.BitLength;

            do
            {
                RandomValue = BigInteger.Random(bitLength);

                ClientExchangeValue = BigInteger.ModPow(Group, RandomValue, Prime);

            } while (ClientExchangeValue < 1 || ClientExchangeValue > ((Prime - 1)));
        }

        /// <summary>
        /// Handles the server DH reply message.
        /// </summary>
        /// <param name="hostKey">The host key.</param>
        /// <param name="serverExchangeValue">The server exchange value.</param>
        /// <param name="signature">The signature.</param>
        protected virtual void HandleServerDhReply(byte[] hostKey, BigInteger serverExchangeValue, byte[] signature)
        {
            ServerExchangeValue = serverExchangeValue;
            HostKey = hostKey;
            SharedKey = BigInteger.ModPow(serverExchangeValue, RandomValue, Prime);
            Signature = signature;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Cryptography
{
    public class DiffieHellman
    {
        private BigInteger p = 0;
        private BigInteger g = 0;
        private BigInteger a = 0;
        private BigInteger b = 0;
        private BigInteger s = 0;
        private BigInteger A = 0;
        private BigInteger B = 0;

        public BigInteger GetKey() { return s; }
        public BigInteger GetRequest() { return A; }
        public BigInteger GetResponse() { return A; }

        public String Key { get { return s.ToHexString(); } }

        public override String ToString() { return s.ToHexString(); }
        public Byte[] ToBytes() { return s.getBytes(); }

        /// <summary>
        /// Create a new Diffie-Hellman exchange where the prime number is p and the base is g.
        /// </summary>
        public DiffieHellman(String p, String g)
        {
            this.p = new BigInteger(p, 16);
            this.g = new BigInteger(g, 16);
        }

        ~DiffieHellman() { }

        /// <summary>
        /// Generates the server request and return the A key.
        /// </summary>
        public String GenerateRequest()
        {
            a = BigInteger.genPseudoPrime(256, 30, new Random());
            A = g.modPow(a, p);

            return A.ToHexString();
        }

        /// <summary>
        /// Generates the client response and the S key with the A key.
        /// The B key will be returned.
        /// </summary>
        public String GenerateResponse(String PubKey)
        {
            b = BigInteger.genPseudoPrime(256, 30, new Random());
            B = g.modPow(b, p);

            A = new BigInteger(PubKey, 16);
            s = A.modPow(b, p);

            return B.ToHexString();
        }

        /// <summary>
        /// Handles the client response to generate the S key with the B key.
        /// </summary>
        public void HandleResponse(String PubKey)
        {
            B = new BigInteger(PubKey, 16);
            s = B.modPow(a, p);
        }
    }
}

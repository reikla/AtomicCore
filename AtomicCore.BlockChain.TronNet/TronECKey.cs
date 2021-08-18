﻿using System;
using System.Globalization;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron ECKey
    /// </summary>
    public class TronECKey
    {
        #region Variables

        private readonly ECKey _ecKey;
        private string _publicAddress = null;
        private readonly TronNetwork _network = TronNetwork.MainNet;
        private string _privateKeyHex = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="network">network Type Enum</param>
        public TronECKey(string privateKey, TronNetwork network)
        {
            _ecKey = new ECKey(privateKey.HexToByteArray(), true);
            _network = network;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vch"></param>
        /// <param name="isPrivate"></param>
        /// <param name="network">network Type Enum</param>
        public TronECKey(byte[] vch, bool isPrivate, TronNetwork network)
        {
            _ecKey = new ECKey(vch, isPrivate);
            _network = network;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ecKey"></param>
        /// <param name="network">network Type Enum</param>
        internal TronECKey(ECKey ecKey, TronNetwork network)
        {
            _ecKey = ecKey;
            _network = network;
        }

        /// <summary>
        /// Constructor(new ECKey instane)
        /// </summary>
        /// <param name="network">network Type Enum</param>
        internal TronECKey(TronNetwork network)
        {
            _ecKey = new ECKey();
            _network = network;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static TronECKey GenerateKey(TronNetwork network)
        {
            return new TronECKey(network);
        }

        /// <summary>
        /// Get Public Address
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static string GetPublicAddress(string privateKey, TronNetwork network = TronNetwork.MainNet)
        {
            var key = new TronECKey(privateKey.HexToByteArray(), true, network);

            return key.GetPublicAddress();
        }

        /// <summary>
        /// tron address to eth address
        /// </summary>
        /// <param name="tronAddress">tron address</param>
        /// <param name="isUpper">upper -> true,lower -> false</param>
        /// <returns></returns>
        public static string ConvertToEthAddress(string tronAddress, bool isUpper = false)
        {
            if (string.IsNullOrEmpty(tronAddress))
                throw new ArgumentNullException(nameof(tronAddress));

            byte[] tronAddressBytes = Base58Encoder.DecodeFromBase58Check(tronAddress);
            byte[] addrByte20 = new byte[20];
            Array.Copy(tronAddressBytes, 1, addrByte20, 0, 20);

            string address = addrByte20.ToHex();
            byte[] hash = addrByte20.ToKeccakHash();
            string addressHash = hash.ToHex();

            StringBuilder checksumAddress = new StringBuilder("0x");
            for (var i = 0; i < address.Length; i++)
                if (int.Parse(addressHash[i].ToString(), NumberStyles.HexNumber) > 7)
                    checksumAddress.Append(address[i].ToString().ToUpper());
                else
                    checksumAddress.Append(address[i]);

            if (isUpper)
                return checksumAddress.ToString().ToUpper();
            else
                return checksumAddress.ToString().ToLower();
        }

        /// <summary>
        /// eth address to tron address
        /// </summary>
        /// <param name="ethAddress"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static string ConvertToTronAddress(string ethAddress, TronNetwork network = TronNetwork.MainNet)
        {
            if (string.IsNullOrEmpty(ethAddress))
                throw new ArgumentNullException(nameof(ethAddress));

            byte[] addrByte20 = ethAddress.RemoveHexPrefix().HexToByteArray();

            byte[] address = new byte[21];
            Array.Copy(addrByte20, 0, address, 1, 20);
            address[0] = (byte)network;

            byte[] hash = Base58Encoder.TwiceHash(address);
            byte[] bytes = new byte[4];
            Array.Copy(hash, bytes, 4);

            byte[] addressChecksum = new byte[25];
            Array.Copy(address, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            string tronAddress;
            switch (network)
            {
                case TronNetwork.MainNet:
                    tronAddress = Base58Encoder.Encode(addressChecksum);
                    break;
                default:
                    tronAddress = addressChecksum.ToHex();
                    break;
            }

            return tronAddress;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Public Address Prefix
        /// </summary>
        /// <returns></returns>
        public byte GetPublicAddressPrefix()
        {
            return (byte)_network;
        }

        /// <summary>
        /// Get Public Key
        /// </summary>
        /// <returns></returns>
        public byte[] GetPubKey()
        {
            return _ecKey.GetPubKey();
        }

        /// <summary>
        /// Get Private Key Hex String
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(_privateKeyHex))
                _privateKeyHex = _ecKey.PrivateKey.D.ToByteArrayUnsigned().ToHex();

            return _privateKeyHex;
        }

        /// <summary>
        /// Get Public Address
        /// </summary>
        /// <returns></returns>
        public string GetPublicAddress()
        {
            if (!string.IsNullOrWhiteSpace(_publicAddress)) return _publicAddress;

            byte[] initAddress = _ecKey.GetPubKeyNoPrefix().ToKeccakHash();
            byte[] address = new byte[initAddress.Length - 11];
            Array.Copy(initAddress, 12, address, 1, 20);
            address[0] = GetPublicAddressPrefix();

            byte[] hash = Base58Encoder.TwiceHash(address);
            byte[] bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            byte[] addressChecksum = new byte[25];
            Array.Copy(address, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            if (_network == TronNetwork.MainNet)
                _publicAddress = Base58Encoder.Encode(addressChecksum);
            else
                _publicAddress = addressChecksum.ToHex();

            return _publicAddress;
        }

        #endregion
    }
}
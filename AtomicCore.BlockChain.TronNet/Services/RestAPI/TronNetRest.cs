﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Rest API Implementation Class
    /// </summary>
    public class TronNetRest : ITronNetRest
    {
        #region Variables

        private const string c_apiKeyName = "TRON-PRO-API-KEY";
        private readonly IOptions<TronNetOptions> _options;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public TronNetRest(IOptions<TronNetOptions> options)
        {
            _options = options;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create Full Node Rest Url
        /// </summary>
        /// <param name="actionUrl">接口URL</param>
        /// <returns></returns>
        private string CreateFullNodeRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}{1}",
                this._options.Value.FullNodeRestAPI,
                actionUrl
            );
        }

        /// <summary>
        /// Create Solidity Node Rest Url
        /// </summary>
        /// <param name="actionUrl"></param>
        /// <returns></returns>
        private string CreateSolidityNodeRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}{1}",
                this._options.Value.SolidityNodeRestAPI,
                actionUrl
            );
        }

        /// <summary>
        /// Rest Get Json Result
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RestGetJson(string url)
        {
            string resp;
            try
            {
                //head api key
                Dictionary<string, string> heads = new Dictionary<string, string>()
                {
                    { "Accept","application/json"}
                };

                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads.Add(c_apiKeyName, apiKey);

                resp = HttpProtocol.HttpGet(url, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// Rest Post Json Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RestPostJson(string url)
        {
            string resp;
            try
            {
                //head api key
                Dictionary<string, string> heads = null;
                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads = new Dictionary<string, string>()
                    {
                        { c_apiKeyName,apiKey}
                    };

                resp = HttpProtocol.HttpPost(url, null, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// Rest Post Json Request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">rest url</param>
        /// <param name="json">json data</param>
        /// <returns></returns>
        private string RestPostJson<T>(string url, T json)
        {
            string resp;
            try
            {
                //post data
                string post_data = Newtonsoft.Json.JsonConvert.SerializeObject(json);

                //head api key
                Dictionary<string, string> heads = null;
                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads = new Dictionary<string, string>()
                    {
                        { c_apiKeyName,apiKey}
                    };

                resp = HttpProtocol.HttpPost(url, post_data, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// StringJson Parse to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private T ObjectParse<T>(string resp)
            where T : TronNetValidRestJson, new()
        {
            if (resp.Contains("\"Error\""))
            {
                JObject errorJson = JObject.Parse(resp);
                if (errorJson.ContainsKey("Error"))
                {
                    return new T()
                    {
                        Error = errorJson.GetValue("Error").ToString()
                    };
                }
            }

            T jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        #endregion

        #region ITronAddressUtilitiesRest

        /// <summary>
        /// Generates a random private key and address pair. 
        /// Risk Warning : there is a security risk. 
        /// This interface service has been shutdown by the Trongrid. 
        /// Please use the offline mode or the node deployed by yourself.
        /// </summary>
        /// <returns>Returns a private key, the corresponding address in hex,and base58</returns>
        [Obsolete("Remote service has been removed")]
        public TronNetAddressKeyPairRestJson GenerateAddress()
        {
            string url = CreateFullNodeRestUrl("/wallet/generateaddress");
            string resp = this.RestGetJson(url);
            TronNetAddressKeyPairRestJson restJson = ObjectParse<TronNetAddressKeyPairRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Create address from a specified password string (NOT PRIVATE KEY)
        /// Risk Warning : there is a security risk. 
        /// This interface service has been shutdown by the Trongrid. 
        /// Please use the offline mode or the node deployed by yourself.
        /// </summary>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetAddressBase58CheckRestJson CreateAddress(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));

            string passphraseHex = Encoding.UTF8.GetBytes(passphrase).ToHex();
            string url = CreateFullNodeRestUrl("/wallet/createaddress");
            string resp = this.RestPostJson(url, new { value = passphraseHex });
            TronNetAddressBase58CheckRestJson restJson = ObjectParse<TronNetAddressBase58CheckRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Validates address, returns either true or false.
        /// </summary>
        /// <param name="address">Tron Address</param>
        /// <returns></returns>
        public TronNetAddressValidRestJson ValidateAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            string addressHex = Base58Encoder.DecodeFromBase58Check(address).ToHex();
            string url = CreateFullNodeRestUrl("/wallet/validateaddress");
            string resp = this.RestPostJson(url, new { address = addressHex });
            TronNetAddressValidRestJson restJson = ObjectParse<TronNetAddressValidRestJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronTransactionsRest

        /// <summary>
        /// Get Transaction Sign
        /// Offline Signature
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="createTransaction"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetSignedTransactionRestJson GetTransactionSign(string privateKey, TronNetCreateTransactionRestJson createTransaction)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentNullException(nameof(privateKey));
            if (null == createTransaction)
                throw new ArgumentException("createTransaction is null");

            /* Restore ECKey From Private Key */
            ECKey ecKey = new ECKey(privateKey.HexToByteArray(), true);

            /* hash sign */
            string raw_json = Newtonsoft.Json.JsonConvert.SerializeObject(createTransaction.RawData);
            byte[] raw_data_bytes = Encoding.UTF8.GetBytes(raw_json);
            byte[] hash_bytes = raw_data_bytes.ToSHA256Hash();
            byte[] sign_bytes = ecKey.Sign(hash_bytes).ToByteArray();
            string sign = sign_bytes.ToHex();

            TronNetSignedTransactionRestJson restJson = new TronNetSignedTransactionRestJson()
            {
                TxID = createTransaction.TxID,
                RawData = createTransaction.RawData,
                RawDataHex = createTransaction.RawDataHex,
                Visible = createTransaction.Visible,
                Signature = new string[] { sign }
            };
            restJson.Signature = new string[] { sign };

            return restJson;

            ////////create request data
            //////dynamic reqData = new
            //////{
            //////    transaction = new 
            //////    {
            //////        txID = createTransaction.TxID,
            //////        raw_data = createTransaction.RawData
            //////    },
            //////    privateKey
            //////};

            //////string url = CreateFullNodeRestUrl("/wallet/gettransactionsign");
            //////string resp = this.RestPostJson(url, reqData);
            //////TronNetSignedTransactionRestJson restJson = ObjectParse<TronNetSignedTransactionRestJson>(resp);

            //////return restJson;
        }

        /// <summary>
        /// Broadcast Transaction
        /// </summary>
        /// <param name="singedTransaction"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetResultJson BroadcastTransaction(TronNetSignedTransactionRestJson singedTransaction)
        {
            //create request data
            dynamic reqData = new
            {
                signature = singedTransaction.Signature,
                txID = singedTransaction.TxID,
                raw_data = singedTransaction.RawData
            };

            string url = CreateFullNodeRestUrl("/wallet/broadcasttransaction");
            string resp = this.RestPostJson(url, reqData);
            TronNetResultJson restJson = ObjectParse<TronNetResultJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Broadcast Hex
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetResultJson BroadcastHex(string hex)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Easy Transfer
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetEasyTransferJson EasyTransfer(string passPhrase, string toAddress, ulong amount)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Easy Transfer By Private
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetEasyTransferJson EasyTransferByPrivate(string privateKey, string toAddress, ulong amount)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Create a TRX transfer transaction. 
        /// If to_address does not exist, then create the account on the blockchain.
        /// </summary>
        /// <param name="ownerAddress">To_address is the transfer address</param>
        /// <param name="toAddress">Owner_address is the transfer address</param>
        /// <param name="amount">Amount is the transfer amount,the unit is trx</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson CreateTransaction(string ownerAddress, string toAddress, decimal amount, int? permissionID = null, bool? visible = null)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (amount <= decimal.Zero)
                throw new ArgumentException("amount must be greater than zero");

            //address to hex
            string hex_owner_address = TronNetECKey.ConvertToHexAddress(ownerAddress);
            string hex_to_address = TronNetECKey.ConvertToHexAddress(toAddress);
            long trx_of_sun = (long)(amount * 1000000);

            //create request data
            dynamic reqData = new
            {
                owner_address = hex_owner_address,
                to_address = hex_to_address,
                amount = trx_of_sun
            };
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;
            if (null != visible)
                reqData.visible = visible.Value;

            string url = CreateFullNodeRestUrl("/wallet/createtransaction");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronQueryNetworkRestAPI

        /// <summary>
        /// Get Block by Number
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public TronNetBlockJson GetBlockByNum(ulong blockHeight)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbynum");
            string resp = this.RestPostJson(url, new { num = blockHeight });
            TronNetBlockJson restJson = ObjectParse<TronNetBlockJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Block By Hash(ID)
        /// </summary>
        /// <param name="blockID"></param>
        /// <returns></returns>
        public TronNetBlockJson GetBlockById(string blockID)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentNullException(nameof(blockID));

            string url = CreateFullNodeRestUrl("/wallet/getblockbyid");
            string resp = this.RestPostJson(url, new { value = blockID });
            TronNetBlockJson restJson = ObjectParse<TronNetBlockJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get a list of block objects by last blocks
        /// </summary>
        /// <param name="lastNum"></param>
        /// <returns></returns>
        public TronNetBlockListJson GetBlockByLatestNum(ulong lastNum)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbylatestnum");
            string resp = this.RestPostJson(url, new { num = lastNum });
            TronNetBlockListJson restJson = ObjectParse<TronNetBlockListJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Returns the list of Block Objects included in the 'Block Height' range specified.
        /// </summary>
        /// <param name="startNum">Starting block height, including this block.</param>
        /// <param name="endNum">Ending block height, excluding that block.</param>
        /// <returns></returns>
        public TronNetBlockListJson GetBlockByLimitNext(ulong startNum, ulong endNum)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbylimitnext");
            string resp = this.RestPostJson(url, new { startNum, endNum });
            TronNetBlockListJson restJson = ObjectParse<TronNetBlockListJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Query the latest block information
        /// </summary>
        /// <returns></returns>
        public TronNetBlockDetailsJson GetNowBlock()
        {
            string url = CreateFullNodeRestUrl("/wallet/getnowblock");
            string resp = this.RestPostJson(url);
            TronNetBlockDetailsJson restJson = ObjectParse<TronNetBlockDetailsJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Transaction By Txid
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        public TronNetTransactionRestJson GetTransactionByID(string txid)
        {
            if (string.IsNullOrEmpty(txid))
                throw new ArgumentNullException(nameof(txid));

            string url = CreateFullNodeRestUrl("/wallet/gettransactionbyid");
            string resp = this.RestPostJson(url, new { value = txid });
            TronNetTransactionRestJson restJson = ObjectParse<TronNetTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Transaction Info By Txid
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        public TronNetTransactionInfoJson GetTransactionInfoById(string txid)
        {
            if (string.IsNullOrEmpty(txid))
                throw new ArgumentNullException(nameof(txid));

            string url = CreateFullNodeRestUrl("/wallet/gettransactioninfobyid");
            string resp = this.RestPostJson(url, new { value = txid });
            TronNetTransactionInfoJson restJson = ObjectParse<TronNetTransactionInfoJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get TransactionInfo By BlockHeight
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        [System.Obsolete("Please use method 'GetBlockByLimitNext' instead")]
        public TronNetTransactionInfoJson GetTransactionInfoByBlockNum(ulong blockHeight)
        {
            throw new NotImplementedException("Please use method 'GetBlockByLimitNext' instead");
        }

        /// <summary>
        /// Return List of Nodes
        /// </summary>
        /// <returns></returns>
        public TronNetNodeJson ListNodes()
        {
            string url = CreateFullNodeRestUrl("/wallet/listnodes");
            string resp = this.RestGetJson(url);
            TronNetNodeJson restJson = ObjectParse<TronNetNodeJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get NodeInfo
        /// </summary>
        /// <returns></returns>
        public TronNetNodeOverviewJson GetNodeInfo()
        {
            string url = CreateFullNodeRestUrl("/wallet/getnodeinfo");
            string resp = this.RestGetJson(url);
            TronNetNodeOverviewJson restJson = ObjectParse<TronNetNodeOverviewJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Chain Parameters
        /// </summary>
        /// <returns></returns>
        public TronNetChainParameterOverviewJson GetChainParameters()
        {
            string url = CreateFullNodeRestUrl("/wallet/getchainparameters");
            string resp = this.RestGetJson(url);
            TronNetChainParameterOverviewJson restJson = ObjectParse<TronNetChainParameterOverviewJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Block's Account Balance Change
        /// 47.241.20.47 & 161.117.85.97 &161.117.224.116 &161.117.83.38
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="blockHeight"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetBlockBalanceJson GetBlockBalance(string blockHash, ulong blockHeight, bool visible = true)
        {
            if (string.IsNullOrEmpty(blockHash))
                throw new ArgumentNullException(nameof(blockHash));
            if (blockHeight <= 0)
                throw new ArgumentException("blockHeight must be greater than zero");

            string url = CreateFullNodeRestUrl("/wallet/getblockbalance");
            string resp = this.RestPostJson(url, new { hash = blockHash, number = blockHeight, visible });
            TronNetBlockBalanceJson restJson = ObjectParse<TronNetBlockBalanceJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronNetTRC10TokenRest

        /// <summary>
        /// Get Asset Issue By Account
        /// </summary>
        /// <param name="tronAddress"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetAssetIssueByAccount(string tronAddress, bool? visible = null)
        {
            string address = TronNetECKey.ConvertToHexAddress(tronAddress);

            //create request data
            dynamic reqData = new
            {
                address
            };
            if (null != visible)
                reqData.visible = visible.Value;

            string url = CreateFullNodeRestUrl("/wallet/getassetissuebyaccount");
            string resp = this.RestPostJson(url, reqData);
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get AssetIssue By ID
        /// </summary>
        /// <param name="assertID"></param>
        /// <returns></returns>
        public TronNetAssetInfoJson GetAssetIssueById(int assertID)
        {
            string url = CreateFullNodeRestUrl("/wallet/getassetissuebyid");
            string resp = this.RestPostJson(url, new { value = assertID });
            TronNetAssetInfoJson restJson = ObjectParse<TronNetAssetInfoJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get AssetIssue List
        /// </summary>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetAssetIssueList()
        {
            string url = CreateFullNodeRestUrl("/wallet/getassetissuelist");
            string resp = this.RestGetJson(url);
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Paginated AssetIssue List
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetPaginatedAssetIssueList(int offset, int limit)
        {
            string url = CreateFullNodeRestUrl("/wallet/getpaginatedassetissuelist");
            string resp = this.RestPostJson(url, new { offset, limit });
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Create AssetIssue
        /// </summary>
        /// <param name="ownerAddress">Owner Address</param>
        /// <param name="tokenName">Token Name</param>
        /// <param name="tokenPrecision">Token Precision</param>
        /// <param name="tokenAbbr">Token Abbr</param>
        /// <param name="totalSupply">Token Total Supply</param>
        /// <param name="trxNum">
        /// Define the price by the ratio of trx_num/num(The unit of 'trx_num' is SUN)
        /// </param>
        /// <param name="num">
        /// Define the price by the ratio of trx_num/num
        /// </param>
        /// <param name="startTime">ICO StartTime</param>
        /// <param name="endTime">ICO EndTime</param>
        /// <param name="tokenDescription">Token Description</param>
        /// <param name="tokenUrl">Token Official Website Url</param>
        /// <param name="freeAssetNetLimit">Token Free Asset Net Limit</param>
        /// <param name="publicFreeAssetNetLimit">Token Public Free Asset Net Limit</param>
        /// <param name="frozenSupply">Token Frozen Supply</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson CreateAssetIssue(string ownerAddress, string tokenName, int tokenPrecision, string tokenAbbr, ulong totalSupply, ulong trxNum, ulong num, DateTime startTime, DateTime endTime, string tokenDescription, string tokenUrl, ulong freeAssetNetLimit, ulong publicFreeAssetNetLimit, TronNetFrozenSupplyJson frozenSupply)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(tokenName))
                throw new ArgumentNullException(nameof(tokenName));
            if (tokenPrecision <= 0)
                tokenPrecision = 0;
            if (string.IsNullOrEmpty(tokenAbbr))
                throw new ArgumentNullException(nameof(tokenAbbr));
            if (totalSupply <= 0)
                throw new ArgumentException("'totalSupply' must be greater to zero");
            if (string.IsNullOrEmpty(tokenUrl))
                tokenUrl = string.Empty;
            if (string.IsNullOrEmpty(tokenDescription))
                tokenDescription = string.Empty;
            if (null == frozenSupply)
                throw new ArgumentNullException(nameof(frozenSupply));

            string hex_owner_address = TronNetECKey.ConvertToHexAddress(ownerAddress);
            string hex_token_name = TronNetUntils.StringToHexString(tokenName, true, Encoding.UTF8);
            string hex_token_abbr = TronNetUntils.StringToHexString(tokenAbbr, true, Encoding.UTF8);
            long start_time = TronNetUntils.LocalDatetimeToMillisecondTimestamp(startTime);
            long end_time = TronNetUntils.LocalDatetimeToMillisecondTimestamp(endTime);

            string hex_token_desc = TronNetUntils.StringToHexString(tokenDescription, true, Encoding.UTF8);
            string hex_url = TronNetUntils.StringToHexString(tokenUrl, true, Encoding.UTF8);

            //create request data
            dynamic reqData = new
            {
                owner_address = hex_owner_address,
                name = hex_token_name,
                precision = tokenPrecision,
                abbr = hex_token_abbr,
                total_supply = totalSupply,
                trx_num = trxNum,
                num,
                start_time,
                end_time,
                description = hex_token_desc,
                url = hex_url,
                free_asset_net_limit = freeAssetNetLimit,
                public_free_asset_net_limit = publicFreeAssetNetLimit,
                frozen_supply = frozenSupply
            };

            string url = CreateFullNodeRestUrl("/wallet/createassetissue");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        #endregion
    }
}

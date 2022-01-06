﻿namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscTokens Interface
    /// </summary>
    public interface IBscTokens
    {
        /// <summary>
        /// Returns the total supply of a BEP-20 token.
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetBEP20TotalSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the current circulating supply of a BEP-20 token. 
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetBEP20CirculatingSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the current balance of a BEP-20 token of an address.
        /// </summary>
        /// <param name="address">the string representing the address to check for token balance</param>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetBEP20BalanceOf(string address, string contractaddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}

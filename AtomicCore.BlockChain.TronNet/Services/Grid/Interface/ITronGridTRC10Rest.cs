﻿namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid TRC10 Rest
    /// https://cn.developers.tron.network/reference/list-all-assets-trc10-tokens-on-chain
    /// </summary>
    public interface ITronGridTRC10Rest
    {
        /// <summary>
        /// Get a list of all TRC10s
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        TronGridRestResult<TronGridAssetTrc10Info> GetTrc10List(TronGridAssetTrc10Query query = null);
    }
}

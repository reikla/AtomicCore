﻿namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// cache expiration mode
    /// </summary>
    public enum BlockCypherCacheMode
    {
        /// <summary>
        /// none
        /// </summary>
        None = 0,

        /// <summary>
        /// absolute expired
        /// </summary>
        AbsoluteExpired = 1,

        /// <summary>
        /// slide expired
        /// </summary>
        SlideExpired = 2
    }
}
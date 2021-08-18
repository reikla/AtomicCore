﻿namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Client Interface
    /// </summary>
    public interface ITronClient
    {
        /// <summary>
        /// Tron Network Type Enum
        /// </summary>
        TronNetwork TronNetwork { get; }

        /// <summary>
        /// Get Rest Api
        /// </summary>
        /// <returns></returns>
        ITronRestAPI GetRestAPI();

        /// <summary>
        /// Grpc Channel Client
        /// </summary>
        /// <returns></returns>
        IGrpcChannelClient GetChannel();

        /// <summary>
        /// Get Wallet Interface Instance
        /// </summary>
        /// <returns></returns>
        IWalletClient GetWallet();

        /// <summary>
        /// Get Transaction Interface Instance
        /// </summary>
        /// <returns></returns>
        ITransactionClient GetTransaction();
    }
}
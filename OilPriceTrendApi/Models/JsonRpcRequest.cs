namespace OilPriceTrendApi.Models
{
    /// <summary>
    /// Represents a JSON-RPC request.
    /// </summary>
    /// <typeparam name="T">The type of the parameters.</typeparam>
    public class JsonRpcRequest<T>
    {
        /// <summary>
        /// Gets or sets the version of JSON-RPC.
        /// </summary>
        public string? Jsonrpc { get; set; }

        /// <summary>
        /// Gets or sets the method to be invoked.
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the method.
        /// </summary>
        public T? Params { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the request.
        /// </summary>
        public int Id { get; set; }
    }
}
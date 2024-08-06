namespace OilPriceTrendApi.Models
{
    /// <summary>
    /// Represents a JSON-RPC response.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonRpcResponse<T>
    {
        /// <summary>
        /// Gets or sets the version of JSON-RPC.
        /// </summary>
        public string Jsonrpc { get; set; } = "2.0";

        /// <summary>
        /// Gets or sets the unique identifier for the response.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the result of the method invocation.
        /// </summary>
        public T? Result { get; set; }

        /// <summary>
        /// Gets or sets the error information, if any.
        /// </summary>
        public object? Error { get; set; }
    }
}
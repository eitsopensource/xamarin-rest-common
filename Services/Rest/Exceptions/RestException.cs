using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamarinrest.Services.Rest.Exceptions
{
    public class RestException
    {
        /// <summary>
        /// Data do erro
        /// </summary>
        [JsonProperty("Timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// HTTP status CODE
        /// </summary>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Tipo da exception
        /// </summary>
        [JsonProperty("Error")]
        public string Error { get; set; }

        /// <summary>
        /// Mensagem da exception
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; }
        
        /// <summary>
        /// Mensagem da exception
        /// </summary>
        [JsonProperty("Error_Description")]
        public string ErrorDescription { set { Message = value; } }

        /// <summary>
        /// REST Path que deu erro (URI)
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; set; }
    }
}

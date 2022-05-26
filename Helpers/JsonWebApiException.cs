using System;


namespace FirelyClient.Helpers
{
    public class JsonWebApiException : Exception
    {
        public string ResponseBody { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebApiException"/> class.
        /// </summary>
        /// <param name="responseBody">The response body.</param>
        public JsonWebApiException(string responseBody)
        {
            ResponseBody = responseBody;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebApiException"/> class.
        /// </summary>
        /// <param name="responseBody">The response body.</param>
        /// <param name="message">The message.</param>
        public JsonWebApiException(string responseBody, string message) : base(message)
        {
            ResponseBody = responseBody;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebApiException"/> class.
        /// </summary>
        /// <param name="responseBody">The response body.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public JsonWebApiException(string responseBody, string message, Exception exception) : base(message, exception)
        {
            ResponseBody = responseBody;
        }
    }
}

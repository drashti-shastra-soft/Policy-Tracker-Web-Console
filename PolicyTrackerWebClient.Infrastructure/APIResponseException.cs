using System;

namespace PolicyTrackerWebClient.Infrastructure
{
    public class APIResponseException : Exception
    {
        public APIResponseException(string error) : base(error)
        {
        }
    }
}

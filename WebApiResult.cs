using System.Net;

namespace GuidX.WebApiClient
{
    public class WebApiResult<T>
    {
        #region Properties

        public HttpStatusCode HttpStatusCode { get; set; }

        public bool IsSuccessStatusCode => (int) HttpStatusCode >= 200 && (int) HttpStatusCode <= 299;

        public T Value { get; set; }

        #endregion
    }
}
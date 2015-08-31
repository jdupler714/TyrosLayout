using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;


namespace AurisIdeas.XamarinWebServices
{
	/// <summary>
	/// Represents a web service call result returning a String.
	/// </summary>
	public class WebServiceStringResult
	{
		public HttpStatusCode? ResponseCode { get; set; }

		public string ResultText { get; set; }

		public WebServiceStringResult ()
		{
			this.ResponseCode = null;
			this.ResultText = null;
		}

		public WebServiceStringResult (HttpStatusCode code, string text)
		{
			this.ResponseCode = code;
			this.ResultText = text;
		}
	}

	public class WebServices
	{
		#region Enums

		/// <summary>
		/// Http verb options.
		/// </summary>
		public enum HttpVerb
		{
			GET = 1,
			POST = 2,
			PUT = 3,
			DELETE = 4
		}

		/// <summary>
		/// Request data formatting. URLEncoded is usually the best for getting a password-type OAuth token.
		/// </summary>
		public enum RequestDataFormatType
		{
			JSON = 1,
			URLENCODED = 2
		}

		/// <summary>
		/// Response data formatting. Used for determining whether to return JSON or XML or not specify.
		/// </summary>
		public enum ResponseDataFormatType
		{
			JSON = 1,
			XML = 2,
			UNSPECIFIED = 3
		}

		/// <summary>
		/// Request encoding. URLEncoded is usually the best for getting a password-type OAuth token.
		/// </summary>
		public enum RequestEncodingType
		{
			UTF7 = 1,
			UTF8 = 2,
			ASCII = 3
		}

		/// <summary>
		/// Supported authorization header types.
		/// </summary>
		public enum AuthorizationType
		{
			BEARER = 1,
			CUSTOM = 2,
			NOT_APPLICABLE = 3
		}

		#endregion

		#region Properties

		/* endpoint URL */
		private Uri EndpointUri { get; set; }

		private RequestDataFormatType RequestDataFormat { get; set; }

		/* for Accepts header */
		private ResponseDataFormatType ResponseDataFormat { get; set; }

		/* for charset header */
		private RequestEncodingType RequestEncoding { get; set; }

		/* for Authorization header */
		private String AuthenticationToken { get; set; }

		/* for Authorization header */
		private AuthorizationType AuthenticationTokenType { get; set; }

		/* encoded properly as UrlEncoded or JSON in service call */
		private string RequestBody { get; set; }

		private List<HttpRequestHeader> AdditionalHeaders { get; set; }

		#endregion

		#region Constructor

		public WebServices ()
		{
			// Nothing to do here...
		}

		#endregion

		#region Web Service Private Result Retrievers

		/// <summary>
		/// Make a GET call.
		/// </summary>
		/// <returns>WebServiceStringResult. The call result.</returns>
		private async Task<WebServiceStringResult> GetCall ()
		{
			// Make the swap for the real auth token.
			using (var client = new HttpClient ()) {
				// Set up the client.
				client.BaseAddress = this.EndpointUri;
				AddResponseTypeHeader (client);
				AddAuthorizationHeader (client);

				// Get the response.
				using (var response = client.GetAsync (this.EndpointUri).Result) {
					return new WebServiceStringResult (response.StatusCode, await response.Content.ReadAsStringAsync ());
				}
			}
		}

		/// <summary>
		/// Make a POST call.
		/// </summary>
		/// <returns>WebServiceStringResult. The call result.</returns>
		private async Task<WebServiceStringResult> PostCall ()
		{
			using (var client = new HttpClient ()) {
				// Set up the client.
				client.BaseAddress = this.EndpointUri;
				client.DefaultRequestHeaders.Host = this.EndpointUri.DnsSafeHost;
				AddResponseTypeHeader (client);
				AddRequestContentTypeHeader (client);
				AddAuthorizationHeader (client);
				var content = GetAppropriateStringContent (this.RequestBody);

				// Get the response.
				var response = await client.PostAsync (this.EndpointUri, content);
				return new WebServiceStringResult (response.StatusCode, await response.Content.ReadAsStringAsync ());
			}
		}

		/// <summary>
		/// Make a PUT call.
		/// </summary>
		/// <returns>WebServiceStringResult. The call result.</returns>
		private async Task<WebServiceStringResult> PutCall ()
		{
			using (var client = new HttpClient ()) {
				// Set up the client.
				client.BaseAddress = this.EndpointUri;
				AddResponseTypeHeader (client);
				AddRequestContentTypeHeader (client);
				AddAuthorizationHeader (client);
				GetAppropriateStringContent (this.RequestBody);

				// Get the response.
				using (var response = client.PutAsync (this.EndpointUri, GetAppropriateStringContent (this.RequestBody)).Result) {
					return new WebServiceStringResult (response.StatusCode, await response.Content.ReadAsStringAsync ());
				}
			}
		}

		/// <summary>
		/// Make a DELETE call.
		/// </summary>
		/// <returns>WebServiceStringResult. The call result.</returns>
		private async Task<WebServiceStringResult> DeleteCall ()
		{
			using (var client = new HttpClient ()) {
				// Set up the client.
				client.BaseAddress = this.EndpointUri;
				AddResponseTypeHeader (client);
				AddRequestContentTypeHeader (client);
				AddAuthorizationHeader (client);

				// Get the response.
				using (var response = client.DeleteAsync (this.EndpointUri).Result) {
					return new WebServiceStringResult (response.StatusCode, await response.Content.ReadAsStringAsync ());
				}
			}
		}

		#endregion

		#region Header Management Functions

		/// <summary>
		/// Adds the Accept data type header if one has been defined.
		/// </summary>
		/// <returns>The response type header.</returns>
		/// <param name="clientToModify">Client to modify.</param>
		private void AddResponseTypeHeader (HttpClient clientToModify)
		{
			switch (this.ResponseDataFormat) {
			case ResponseDataFormatType.JSON: 
				clientToModify.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				break;
			case ResponseDataFormatType.XML: 
				clientToModify.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/xml"));
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Adds an authorization header to the request if one has been specified.
		/// </summary>
		/// <param name="clientToModify">Client to modify.</param>
		private void AddAuthorizationHeader (HttpClient clientToModify)
		{
			if (!String.IsNullOrWhiteSpace (this.AuthenticationToken)) {
				switch (this.AuthenticationTokenType) {
				case AuthorizationType.BEARER:
					clientToModify.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", this.AuthenticationToken);
					break;
				case AuthorizationType.CUSTOM:
					clientToModify.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("custom", this.AuthenticationToken);
					break;
				}
			}
		}

		/// <summary>
		/// Adds any other headers passed in when creating the class. For example, LinkedIn requires an x-li-json header to 
		/// return JSON if ?json isn't passed in the URL.
		/// </summary>
		/// <returns>The other headers.</returns>
		/// <param name="clientToModify">Client to modify.</param>
		private void AddOtherHeaders (HttpClient clientToModify)
		{
			if (this.AdditionalHeaders != null && this.AdditionalHeaders.Count != 0)
				throw new NotImplementedException ("Add Other Headers: This feature is not yet implemented.");
		}

		/// <summary>
		/// Sets the request content type, telling the endpoint what data format is used for the request body.
		/// </summary>
		/// <param name="clientToModify">Client to modify.</param>
		private void AddRequestContentTypeHeader (HttpClient clientToModify)
		{

			var encodingType = "utf-8"; /* default encoding type */

			switch (this.RequestEncoding) {
			case RequestEncodingType.UTF8:
				encodingType = "utf-8";
				break;
			case RequestEncodingType.UTF7: 
				encodingType = "utf-7";
				break;
			case RequestEncodingType.ASCII: 
				encodingType = "ascii";
				break;
			}

			switch (this.RequestDataFormat) {
			case RequestDataFormatType.JSON: 
				clientToModify.DefaultRequestHeaders.TryAddWithoutValidation ("Content-Type", String.Format ("application/json; charset={0}", encodingType));
				break;
			case RequestDataFormatType.URLENCODED: 
				clientToModify.DefaultRequestHeaders.TryAddWithoutValidation ("Content-Type", String.Format ("www-form-urlencoded; charset={0}", encodingType));
				break;
			}
		}

		#endregion

		#region Public Web Service Functions

		/// <summary>
		/// Gets the string response from the passed endpoint. First converts the request parameters hashtable into a URLEncoded request body.
		/// </summary>
		/// <returns>The string response, if any.</returns>
		/// <param name="url">Endpoint URL.</param>
		/// <param name="verb">HTTP Verb.</param>
		/// <param name="authToken">Auth token, if any.</param>
		/// <param name="requestParameters">Parameters to convert into a URLEncoded string in the request body.</param>
		/// <param name="format">Request Format.</param>
		/// <param name="encoding">Request Encoding.</param>
		/// <param name="additionalHeaders">Additional headers.</param>
		public async Task<WebServiceStringResult> GetStringResponse (
			String url,
			HttpVerb verb,
			String authToken,
			AuthorizationType authType = AuthorizationType.NOT_APPLICABLE,
			Dictionary<String, String> requestParameters = null,
			RequestDataFormatType format = RequestDataFormatType.JSON,
			RequestEncodingType encoding = RequestEncodingType.UTF8,
			List<HttpRequestHeader> additionalHeaders = null
		)
		{
			// Make sure they actually passed parameters for non-GET requests.
			if (verb != HttpVerb.GET && (requestParameters == null || requestParameters.Count == 0))
				throw new ArgumentException ("Request parameter list cannot be null or empty.");

			// Convert the parameters to a URLEncoded request body.
			string requestBody = GetUrlEncodedParameterList (requestParameters);

			// Call the webservice.
			return await GetStringResponse (url, verb, authToken, authType, requestBody, format, encoding, additionalHeaders);

		}
		// eo GetStringResponse

		/// <summary>
		/// Gets the string response from the passed endpoint.
		/// </summary>
		/// <returns>The string response, if any.</returns>
		/// <param name="url">Endpoint URL.</param>
		/// <param name="verb">HTTP Verb.</param>
		/// <param name="authToken">Auth token, if any.</param>
		/// <param name="requestBody">Request body.</param>
		/// <param name="format">Request Format.</param>
		/// <param name="encoding">Request Encoding.</param>
		/// <param name="additionalHeaders">Additional headers.</param>
		public async Task<WebServiceStringResult> GetStringResponse (
			String url,
			HttpVerb verb,
			String authToken,
			AuthorizationType authType = AuthorizationType.NOT_APPLICABLE,
			String requestBody = null,
			RequestDataFormatType format = RequestDataFormatType.JSON,
			RequestEncodingType encoding = RequestEncodingType.UTF8,
			List<HttpRequestHeader> additionalHeaders = null
		)
		{

			// Set up the call details.
			if (String.IsNullOrWhiteSpace (url))
				throw new ArgumentException ("URL cannot be null or whitespace.");
			else
				this.EndpointUri = new Uri (url);
			this.RequestDataFormat = format;
			this.ResponseDataFormat = ResponseDataFormatType.JSON;
			this.RequestEncoding = encoding;
			this.AdditionalHeaders = additionalHeaders;
			this.RequestBody = requestBody;
			if (String.IsNullOrWhiteSpace (authToken)) {
				this.AuthenticationToken = null;
				this.AuthenticationTokenType = AuthorizationType.NOT_APPLICABLE;
			} else {
				this.AuthenticationToken = authToken;
				this.AuthenticationTokenType = authType;
			}

			// Make the call and return the result.
			switch (verb) {
			case HttpVerb.GET:
				return await GetCall ();
			case HttpVerb.POST:
				return await PostCall ();
			case HttpVerb.PUT:
				return await PutCall ();
			case HttpVerb.DELETE:
				return await DeleteCall ();
			default:
				throw new NotImplementedException (String.Format ("Support for verb {0} is not yet implemented.", verb.ToString ()));
			}

		}
		// eo GetStringResponse

		#endregion

		#region Utility

		/// <summary>
		/// Gets a byte representation of the passed string in the passed encoding type.
		/// </summary>
		/// <returns>The bytes.</returns>
		/// <param name="encoding">Encoding type.</param>
		/// <param name="stringToEncode">String to encode.</param>
		private byte[] GetBytes (RequestEncodingType encoding, String stringToEncode)
		{
			switch (encoding) {
			case RequestEncodingType.UTF8:
				return new System.Text.UTF8Encoding ().GetBytes (stringToEncode);
//			case RequestEncodingType.UTF7:
//				return new System.Text.UTF7Encoding().GetBytes(stringToEncode);
//			case RequestEncodingType.ASCII:
//				return new System.Text.ASCIIEncoding().GetBytes(stringToEncode);
			default:
				throw new ArgumentException (String.Format ("Encoding type {0} is not supported.", encoding.ToString ()));
			}

		}
		// eo GetBytes

		//		/// <summary>
		//		/// Gets a byte representation of the passed string in the passed encoding type.
		//		/// </summary>
		//		/// <returns>The bytes.</returns>
		//		/// <param name="encoding">Encoding type.</param>
		//		/// <param name="bytes">String to decode.</param>
		//		private JsonValue GetJsonValue(RequestEncoding encoding, Byte[] bytes) {
		//			switch (encoding) {
		//			case RequestEncoding.UTF8:
		//				return JsonValue.Load(new System.IO.StringReader(new System.Text.UTF8Encoding().GetString(bytes)));
		//			case RequestEncoding.UTF7:
		//				return JsonValue.Load(new System.IO.StringReader(new System.Text.UTF7Encoding().GetString(bytes)));
		//			case RequestEncoding.ASCII:
		//				return JsonValue.Load(new System.IO.StringReader(new System.Text.ASCIIEncoding().GetString(bytes)));
		//			default:
		//				throw new ArgumentException (String.Format("Encoding type {0} is not supported.", encoding.ToString()));
		//			}
		//
		//		} // eo GetJsonValue

		/// <summary>
		/// Returns the encoding type string for the request header.
		/// </summary>
		/// <returns>The encoding type string.</returns>
		/// <param name="encoding">Encoding.</param>
		private string GetEncodingTypeString (RequestDataFormatType encoding)
		{
			switch (encoding) {
			case RequestDataFormatType.JSON:
				return "application/json; encoding=utf-8";
			case RequestDataFormatType.URLENCODED:
				return "www-form-urlencoded; encoding=utf-8";
			default:
				throw new ArgumentException (String.Format ("Encoding type {0} is not supported.", encoding.ToString ()));
			}
		}
		// eo GetEncodingTypeString

		/// <summary>
		/// Given a dictionary (hashtable) of values, returns a parameterized, URLencoded list.
		/// </summary>
		/// <returns>The URL encoded parameter list.</returns>
		/// <param name="d">D.</param>
		private String GetUrlEncodedParameterList (Dictionary<String, String> d)
		{
			StringBuilder stb = new StringBuilder ();

			if (d == null)
				return String.Empty;

			// If a "grant_type" key is included, load that first due to weird .NET grant type requirements.
			if (d.ContainsKey ("grant_type")) {
				stb.Append (String.Format ("{0}={1}&", HttpUtility.UrlEncode ("grant_type"), HttpUtility.UrlEncode ((string)d ["grant_type"])));
				d.Remove ("grant_type"); // so we don't process it twice
			}

			// Continue processing.
			foreach (var key in d.Keys.OfType<String>()) {
				stb.Append (String.Format ("{0}={1}&", HttpUtility.UrlEncode (key), HttpUtility.UrlEncode ((string)d [key])));
			}

			return stb.Length > 1 ? stb.ToString ().Substring (0, stb.Length - 1) : null;
		}

		/// <summary>
		/// Converts this class' encoding to the regular .NET encoding type.
		/// </summary>
		/// <returns>The dot net encoding.</returns>
		private Encoding GetDotNetEncoding (RequestEncodingType encoding)
		{
			switch (encoding) {
			case RequestEncodingType.UTF8:
				return Encoding.UTF8;
			default:
				throw new ArgumentException (String.Concat ("Encoding type ", this.RequestEncoding.ToString (), " not supported."));	
			}
		}

		/// <summary>
		/// Gets the appropriate version of the string content type so we send the right Content-Type header.
		/// </summary>
		/// <returns>The appropriate string content.</returns>
		/// <param name="content">Content.</param>
		private StringContent GetAppropriateStringContent (String content)
		{
			switch (this.ResponseDataFormat) {
			case ResponseDataFormatType.JSON: 
				return new StringContent (content, GetDotNetEncoding (this.RequestEncoding), "application/json");
			case ResponseDataFormatType.XML: 
				return new StringContent (content, GetDotNetEncoding (this.RequestEncoding), "application/xml");
			default:
				return new StringContent (content, GetDotNetEncoding (this.RequestEncoding));
			}

		}

		#endregion

	}
}

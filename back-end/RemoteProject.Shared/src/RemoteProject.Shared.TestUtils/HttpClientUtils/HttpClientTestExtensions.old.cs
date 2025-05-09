using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


#nullable enable
namespace RemoteProject.Shared.TestUtils.HttpClientUtils;

public static class HttpClientTestExtensions
{
  private static readonly HttpStatusCode[] OkOnlyValidStatusCode = new HttpStatusCode[1]
  {
    HttpStatusCode.OK
  };
  private static readonly HttpStatusCode[] OkAndCreatedValidStatusCode = new HttpStatusCode[2]
  {
    HttpStatusCode.OK,
    HttpStatusCode.Created
  };
  public static readonly JsonSerializerOptions? JsonSerializerOptions = new JsonSerializerOptions()
  {
    PropertyNameCaseInsensitive = false,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static bool IsLegacyToken { get; set; }

  public static Func<HttpRequestMessage, HttpRequestMessage>? TokenWriter { get; set; }

  public static Func<string, Type, object>? UserJsonDeserializer { get; set; }

  public static Func<object, string>? UserJsonSerializer { get; set; }

  public static async Task<string> GetText(
    this HttpClient client,
    HttpRequestMessage request,
    string? token,
    CancellationToken cancellationToken,
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    await response.EnsureStatusCode(validStatusCodes == null || validStatusCodes.Length == 0 ? HttpClientTestExtensions.OkOnlyValidStatusCode : validStatusCodes);
    string text = await response.Content.ReadAsStringAsync(cancellationToken);
    response = (HttpResponseMessage) null;
    return text;
  }

  public static async Task<string> GetText(
    this HttpClient client,
    string url,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.GetText(new HttpRequestMessage(HttpMethod.Get, url).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpResponseMessage> Get(
    this HttpClient client,
    HttpRequestMessage request,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    HttpResponseMessage response1 = response;
    HttpStatusCode[] httpStatusCodeArray;
    if (validStatusCodes != null && validStatusCodes.Length != 0)
      httpStatusCodeArray = validStatusCodes;
    else
      httpStatusCodeArray = new HttpStatusCode[1]
      {
        HttpStatusCode.OK
      };
    await response1.EnsureStatusCode(httpStatusCodeArray);
    HttpResponseMessage httpResponseMessage = response;
    response = (HttpResponseMessage) null;
    return httpResponseMessage;
  }

  public static async Task<T> Get<T>(
    this HttpClient client,
    HttpRequestMessage request,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Get(request, token, cancellationToken, validStatusCodes).EnsureReadAsJson<T>(cancellationToken);
  }

  public static async Task<T> Get<T>(
    this HttpClient client,
    string url,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Get<T>(new HttpRequestMessage(HttpMethod.Get, url).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpResponseMessage> Post(
    this HttpClient client,
    HttpRequestMessage request,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    await response.EnsureStatusCode(validStatusCodes == null || validStatusCodes.Length == 0 ? HttpClientTestExtensions.OkAndCreatedValidStatusCode : validStatusCodes);
    HttpResponseMessage httpResponseMessage = response;
    response = (HttpResponseMessage) null;
    return httpResponseMessage;
  }

  public static async Task<T> Post<T>(
    this HttpClient client,
    HttpRequestMessage request,
    string? token,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Post(request, token, cancellationToken, validStatusCodes).EnsureReadAsJson<T>(cancellationToken);
  }

  public static async Task<T> Post<T>(
    this HttpClient client,
    string url,
    object body,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Post<T>(new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpResponseMessage> Post(
    this HttpClient client,
    string url,
    object body,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Post(new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpResponseMessage> Put(
    this HttpClient client,
    HttpRequestMessage request,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    await response.EnsureStatusCode(validStatusCodes == null || validStatusCodes.Length == 0 ? HttpClientTestExtensions.OkOnlyValidStatusCode : validStatusCodes);
    HttpResponseMessage httpResponseMessage = response;
    response = (HttpResponseMessage) null;
    return httpResponseMessage;
  }

  public static async Task<T> Put<T>(
    this HttpClient client,
    HttpRequestMessage request,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Put(request, token, cancellationToken, validStatusCodes).EnsureReadAsJson<T>(cancellationToken);
  }

  public static async Task<HttpResponseMessage> Put(
    this HttpClient client,
    string url,
    object body,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Put(new HttpRequestMessage(HttpMethod.Put, url).WithJsonBody(body).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<T> Put<T>(
    this HttpClient client,
    string url,
    object body,
    string? token = null,
    Dictionary<string, string?>? headers = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return await client.Put<T>(new HttpRequestMessage(HttpMethod.Put, url).WithJsonBody(body).WithHeaders(headers), token, cancellationToken, validStatusCodes);
  }

  public static async Task<string> Create(
    this HttpClient client,
    HttpRequestMessage request,
    string? token,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    HttpResponseMessage response1 = response;
    HttpStatusCode[] httpStatusCodeArray;
    if (validStatusCodes != null && validStatusCodes.Length != 0)
      httpStatusCodeArray = validStatusCodes;
    else
      httpStatusCodeArray = new HttpStatusCode[1]
      {
        HttpStatusCode.Created
      };
    await response1.EnsureStatusCode(httpStatusCodeArray);
    response.Headers.Location.Should().NotBeNull("");
    string str = response.Headers.Location.ToString();
    response = (HttpResponseMessage) null;
    return str;
  }

  public static Task<string> Create(
    this HttpClient client,
    string url,
    object body,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return client.Create(new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpResponseMessage> Delete(
    this HttpClient client,
    HttpRequestMessage request,
    string? token,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    request.WithToken(token);
    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
    HttpResponseMessage response1 = response;
    HttpStatusCode[] httpStatusCodeArray;
    if (validStatusCodes != null && validStatusCodes.Length != 0)
      httpStatusCodeArray = validStatusCodes;
    else
      httpStatusCodeArray = new HttpStatusCode[1]
      {
        HttpStatusCode.OK
      };
    await response1.EnsureStatusCode(httpStatusCodeArray);
    HttpResponseMessage httpResponseMessage = response;
    response = (HttpResponseMessage) null;
    return httpResponseMessage;
  }

  public static Task<HttpResponseMessage> Delete(
    this HttpClient client,
    string url,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken),
    params HttpStatusCode[]? validStatusCodes)
  {
    return client.Delete(new HttpRequestMessage(HttpMethod.Delete, url), token, cancellationToken, validStatusCodes);
  }

  public static async Task<HttpStatusCode> GetStatusCode(
    this HttpClient client,
    HttpMethod method,
    string url,
    object body,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    HttpRequestMessage request = new HttpRequestMessage(method, url);
    request.WithToken(token);
    return (await client.SendAsync(request.WithJsonBody(body), cancellationToken)).StatusCode;
  }

  public static async Task<HttpStatusCode> GetStatusCode(
    this HttpClient client,
    HttpMethod method,
    string url,
    string? token = null,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    HttpRequestMessage request = new HttpRequestMessage(method, url);
    request.WithToken(token);
    return (await client.SendAsync(request, cancellationToken)).StatusCode;
  }

  private static HttpRequestMessage WithToken(this HttpRequestMessage request, string? token)
  {
    if (token != null)
    {
      Func<HttpRequestMessage, HttpRequestMessage> tokenWriter = HttpClientTestExtensions.TokenWriter;
      if (tokenWriter != null)
        request = tokenWriter(request);
      else if (HttpClientTestExtensions.IsLegacyToken)
        request.Headers.Add("X-MATEAPI-AUTH", token);
      else
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    return request;
  }

  public static HttpRequestMessage WithJsonBody(this HttpRequestMessage request, object payload)
  {
    request.Content = (HttpContent) new StringContent(HttpClientTestExtensions.SerializeJson(payload), Encoding.UTF8, "application/json");
    return request;
  }

  public static HttpRequestMessage WithHeaders(
    this HttpRequestMessage request,
    Dictionary<string, string?>? headers)
  {
    if (headers != null)
    {
      foreach (KeyValuePair<string, string> header in headers)
        request.Headers.Add(header.Key, header.Value);
    }
    return request;
  }

  public static async Task<T> EnsureReadAsJson<T>(
    this Task<HttpResponseMessage> response,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    T actualValue = HttpClientTestExtensions.DeserializeJson<T>(await (await response).Content.ReadAsStringAsync(cancellationToken));
    ((object) actualValue).Should().NotBeNull("");
    return actualValue;
  }

  public static async Task<T> EnsureReadAsJson<T>(
    this HttpResponseMessage response,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    T actualValue = HttpClientTestExtensions.DeserializeJson<T>(await response.Content.ReadAsStringAsync(cancellationToken));
    ((object) actualValue).Should().NotBeNull("");
    return actualValue;
  }

  public static async Task<T?> ReadAsJson<T>(
    this HttpResponseMessage response,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    return HttpClientTestExtensions.DeserializeJson<T>(await response.Content.ReadAsStringAsync(cancellationToken));
  }

  public static async Task<T?> ReadAsJson<T>(
    this Task<HttpResponseMessage> response,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    return HttpClientTestExtensions.DeserializeJson<T>(await (await response).Content.ReadAsStringAsync(cancellationToken));
  }

  public static void EnsureStatusCode(
    this HttpStatusCode statusCode,
    params HttpStatusCode[] validStatusCodes)
  {
    ((IEnumerable<HttpStatusCode>) validStatusCodes).Should<HttpStatusCode>().NotBeNull("");
    ((IEnumerable<HttpStatusCode>) validStatusCodes).Should<HttpStatusCode>().Contain(statusCode, "");
  }

  public static async Task EnsureStatusCode(
    this HttpResponseMessage response,
    params HttpStatusCode[] validStatusCodes)
  {
    ((IEnumerable<HttpStatusCode>) validStatusCodes).Should<HttpStatusCode>().NotBeNull("");
    string reason = "Expected one of the [$" + string.Join(",", ((IEnumerable<HttpStatusCode>) validStatusCodes).Select<HttpStatusCode, string>((Func<HttpStatusCode, string>) (x => x.ToString()))) + "] values";
    try
    {
      if (response.Content != null)
        reason = "$" + reason + "\nContent: " + await response.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
    }
    ((IEnumerable<HttpStatusCode>) validStatusCodes).Should<HttpStatusCode>().Contain(response.StatusCode, reason);
    reason = (string) null;
  }

  private static string SerializeJson(object obj)
  {
    Func<object, string> userJsonSerializer = HttpClientTestExtensions.UserJsonSerializer;
    return userJsonSerializer != null ? userJsonSerializer(obj) : JsonSerializer.Serialize<object>(obj, HttpClientTestExtensions.JsonSerializerOptions);
  }

  private static T DeserializeJson<T>(string str)
  {
    Func<string, Type, object> jsonDeserializer = HttpClientTestExtensions.UserJsonDeserializer;
    return jsonDeserializer != null ? (T) jsonDeserializer(str, typeof (T)) : JsonSerializer.Deserialize<T>(str, HttpClientTestExtensions.JsonSerializerOptions);
  }
}
// using System.Net;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Text.Json;
// using FluentAssertions;
//
// namespace RemoteProject.Shared.TestUtils.HttpClientUtils;
//
// public static class HttpClientTestExtensions
// {
//     private static readonly HttpStatusCode[] OkOnlyValidStatusCode = new[] { HttpStatusCode.OK };
//
//     private static readonly HttpStatusCode[] OkAndCreatedValidStatusCode =
//         new[] { HttpStatusCode.OK, HttpStatusCode.Created };
//
//     public static bool IsLegacyToken { get; set; }
//
//     public static Func<string?, HttpRequestMessage, HttpRequestMessage>? TokenWriter { get; set; }
//
//     public static readonly JsonSerializerOptions? JsonSerializerOptions = new()
//     {
//         PropertyNameCaseInsensitive = true,
//         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//     };
//
//     public static Func<string, Type, object>? UserJsonDeserializer { get; set; }
//     public static Func<object, string>? UserJsonSerializer { get; set; }
//
//     public static async Task<string> GetText(
//         this HttpClient client,
//         HttpRequestMessage request,
//         string? token,
//         CancellationToken cancellationToken,
//         params HttpStatusCode[]? validStatusCodes
//     )
//     {
//         request.WithToken(token);
//
//         var response = await client.SendAsync(request, cancellationToken);
//         await response.EnsureStatusCode(
//             validStatusCodes is null || validStatusCodes.Length == 0
//                 ? OkOnlyValidStatusCode
//                 : validStatusCodes
//         );
//
//         var result = await response.Content.ReadAsStringAsync(cancellationToken);
//
//         return result!;
//     }
//
//     public static async Task<string> GetText(this HttpClient client, string url, string? token = null,
//         Dictionary<string, string?>? headers = null, CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes) => await GetText(client,
//         new HttpRequestMessage(HttpMethod.Get, url).WithHeaders(headers), token, cancellationToken,
//         validStatusCodes);
//
//
//     public static async Task<HttpResponseMessage> Send(
//         this HttpClient client,
//         HttpRequestMessage request,
//         string? token = null,
//         CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes
//     )
//     {
//         request.WithToken(token);
//
//         var response = await client.SendAsync(request, cancellationToken);
//         await response.EnsureStatusCode(
//             validStatusCodes is null || validStatusCodes.Length == 0
//                 ? new[] { HttpStatusCode.OK }
//                 : validStatusCodes
//         );
//
//         return response;
//     }
//
//     public static async Task<T> Send<T>(this HttpClient client, HttpRequestMessage request, string? token = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, request, token, cancellationToken, validStatusCodes)
//             .EnsureReadAsJson<T>(cancellationToken);
//
//     public static async Task<HttpResponseMessage> Get(this HttpClient client, HttpRequestMessage request, string? token,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, request, token, cancellationToken, validStatusCodes);
//
//     public static async Task<T> Get<T>(this HttpClient client, HttpRequestMessage request, string? token,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, request, token, cancellationToken, validStatusCodes)
//             .EnsureReadAsJson<T>(cancellationToken);
//
//     public static async Task<T> Get<T>(this HttpClient client, string url, string? token = null,
//         Dictionary<string, string?>? headers = null, CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes) => await Post<T>(client,
//         new HttpRequestMessage(HttpMethod.Get, url).WithHeaders(headers), token,
//         cancellationToken, validStatusCodes);
//
//     public static async Task<HttpResponseMessage> Get(this HttpClient client, string url,
//         string? token = null, Dictionary<string, string?>? headers = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, new HttpRequestMessage(HttpMethod.Get, url).WithHeaders(headers),
//             token, cancellationToken, validStatusCodes);
//
//
//     public static async Task<T> Post<T>(this HttpClient client, HttpRequestMessage request, string? token,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, request, token, cancellationToken, validStatusCodes)
//             .EnsureReadAsJson<T>(cancellationToken);
//
//     public static async Task<T> Post<T>(this HttpClient client, string url, object body, string? token = null,
//         Dictionary<string, string?>? headers = null, CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes) => await Post<T>(client,
//         new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body).WithHeaders(headers), token,
//         cancellationToken, validStatusCodes);
//
//     public static async Task<HttpResponseMessage> Post(this HttpClient client, string url, object body,
//         string? token = null, Dictionary<string, string?>? headers = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body).WithHeaders(headers),
//             token, cancellationToken, validStatusCodes);
//
//     public static async Task<T> Put<T>(this HttpClient client, HttpRequestMessage request, string? token = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) =>
//         await Send(client, request, token, cancellationToken, validStatusCodes)
//             .EnsureReadAsJson<T>(cancellationToken);
//
//     public static async Task<HttpResponseMessage> Put(this HttpClient client, string url, object body,
//         string? token = null, Dictionary<string, string?>? headers = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes
//     ) => await Send(client, new HttpRequestMessage(HttpMethod.Put, url).WithJsonBody(body).WithHeaders(headers),
//         token, cancellationToken, validStatusCodes);
//
//     public static async Task<T> Put<T>(this HttpClient client, string url, object body, string? token = null,
//         Dictionary<string, string?>? headers = null, CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes) => await Send<T>(client,
//         new HttpRequestMessage(HttpMethod.Put, url).WithJsonBody(body).WithHeaders(headers), token,
//         cancellationToken, validStatusCodes);
//
//
//     public static async Task<string> Create(
//         this HttpClient client,
//         HttpRequestMessage request,
//         string? token,
//         CancellationToken cancellationToken = default,
//         params HttpStatusCode[]? validStatusCodes
//     )
//     {
//         request.WithToken(token);
//
//         var response = await client.SendAsync(request, cancellationToken);
//         await response.EnsureStatusCode(
//             validStatusCodes is null || validStatusCodes.Length == 0
//                 ? new[] { HttpStatusCode.Created }
//                 : validStatusCodes
//         );
//
//         response.Headers.Location.Should().NotBeNull();
//
//         return response.Headers.Location!.ToString();
//     }
//
//     public static Task<string> Create(this HttpClient client, string url, object body, string? token = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes) => Create(client,
//         new HttpRequestMessage(HttpMethod.Post, url).WithJsonBody(body), token, cancellationToken,
//         validStatusCodes);
//
//
//     public static Task<HttpResponseMessage> Delete(this HttpClient client, string url, string? token = null,
//         CancellationToken cancellationToken = default, params HttpStatusCode[]? validStatusCodes)
//         => Send(client, new HttpRequestMessage(HttpMethod.Delete, url), token, cancellationToken,
//             validStatusCodes);
//
//     public static async Task<HttpStatusCode> GetStatusCode(
//         this HttpClient client,
//         HttpMethod method,
//         string url,
//         object body,
//         string? token = null,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var request = new HttpRequestMessage(method, url);
//         request.WithToken(token);
//
//         request = request.WithJsonBody(body);
//
//         var response = await client.SendAsync(request, cancellationToken);
//         return response.StatusCode;
//     }
//
//     public static async Task<HttpStatusCode> GetStatusCode(
//         this HttpClient client,
//         HttpMethod method,
//         string url,
//         string? token = null,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var request = new HttpRequestMessage(method, url);
//         request.WithToken(token);
//
//         var response = await client.SendAsync(request, cancellationToken);
//         return response.StatusCode;
//     }
//
//     private static HttpRequestMessage WithToken(
//         this HttpRequestMessage request,
//         string? token
//     )
//     {
//         if (token is not null)
//         {
//             var tokenWriter = TokenWriter;
//             if (tokenWriter is not null)
//             {
//                 request = tokenWriter(token, request);
//             }
//             else if (IsLegacyToken)
//             {
//                 request.Headers.Add("X-MATEAPI-AUTH", token);
//             }
//             else
//             {
//                 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
//             }
//         }
//
//         return request;
//     }
//
//     public static HttpRequestMessage WithJsonBody(
//         this HttpRequestMessage request,
//         object payload
//     )
//     {
//         request.Content = new StringContent(
//             SerializeJson(payload),
//             Encoding.UTF8,
//             "application/json"
//         );
//         return request;
//     }
//
//     public static HttpRequestMessage WithHeaders(
//         this HttpRequestMessage request,
//         Dictionary<string, string?>? headers
//     )
//     {
//         if (headers is not null)
//         {
//             foreach (var header in headers)
//             {
//                 request.Headers.Add(header.Key, header.Value);
//             }
//         }
//
//         return request;
//     }
//
//     public static async Task<T> EnsureReadAsJson<T>(
//         this Task<HttpResponseMessage> response,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var r = await response;
//         var json = await r.Content.ReadAsStringAsync(cancellationToken);
//         var result = DeserializeJson<T>(json);
//
//         result.Should().NotBeNull();
//
//         return result!;
//     }
//
//     public static async Task<T> EnsureReadAsJson<T>(
//         this HttpResponseMessage response,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var json = await response.Content.ReadAsStringAsync(cancellationToken);
//         var result = DeserializeJson<T>(json);
//
//         result.Should().NotBeNull();
//
//         return result!;
//     }
//
//     public static async Task<T?> ReadAsJson<T>(
//         this HttpResponseMessage response,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var json = await response.Content.ReadAsStringAsync(cancellationToken);
//         return DeserializeJson<T>(json);
//     }
//
//     public static async Task<T?> ReadAsJson<T>(
//         this Task<HttpResponseMessage> response,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var r = await response;
//         var json = await r.Content.ReadAsStringAsync(cancellationToken);
//         return DeserializeJson<T>(json);
//     }
//
//     public static void EnsureStatusCode(
//         this HttpStatusCode statusCode,
//         params HttpStatusCode[] validStatusCodes
//     )
//     {
//         validStatusCodes.Should().NotBeNull();
//
//         validStatusCodes.Should().Contain(statusCode);
//     }
//
//     public static async Task EnsureStatusCode(
//         this HttpResponseMessage response,
//         params HttpStatusCode[] validStatusCodes
//     )
//     {
//         validStatusCodes.Should().NotBeNull();
//
//         var reason =
//             $"Expected one of the [${string.Join(",", validStatusCodes.Select(x => x.ToString()))}] values";
//         try
//         {
//             if (response.Content is not null)
//             {
//                 var content = await response.Content.ReadAsStringAsync();
//
//                 reason = $"${reason}\nContent: {content}";
//             }
//         }
//         catch (Exception ex)
//         {
//             reason = $"${reason}\nContent: {ex}";
//         }
//
//         validStatusCodes.Should().Contain(response.StatusCode, reason);
//     }
//
//
//     private static string SerializeJson(object obj)
//     {
//         var serializer = UserJsonSerializer;
//
//         if (serializer is not null)
//         {
//             return serializer(obj);
//         }
//
//         return JsonSerializer.Serialize(obj, JsonSerializerOptions);
//     }
//
//     private static T DeserializeJson<T>(string str)
//     {
//         var deserializer = UserJsonDeserializer;
//
//         if (deserializer is not null)
//         {
//             return (T)deserializer(str, typeof(T));
//         }
//
//         return JsonSerializer.Deserialize<T>(str, JsonSerializerOptions)!;
//     }
// }
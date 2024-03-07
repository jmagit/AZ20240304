// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Owin.Security.Providers.WSO2;

//using(var client = new HttpClient()) {
//    var result = await client.GetAsync("http://localhost:5068/weatherforecast/");
//    Console.WriteLine($"StatusCode: {result.StatusCode} {result.ReasonPhrase}");
//    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
//}

HttpClientHandler clientHandler;

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestVersion = HttpVersion.Version30,
    //DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
    DefaultRequestHeaders = {
        { "Authorization", "Basic YnV0S19iYmdxVm95WkcxY01nak5NWEpDSkN3YTptRnZHZkdwdEp6dG1mSlAzOWZNTzFmNGZEc29h" },
    }
}
) {
    var content = new StringContent("grant_type=client_credentials") { Headers = { ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded") } };
    var result = await client.PostAsync("https://localhost:9443/oauth2/token", content);
    Console.WriteLine($"StatusCode: {result.StatusCode}");
    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
}

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestHeaders = {
        { "Authorization", "Basic R0lYMWE5X01scnRtS3ZhUGJUdHhZNzZkM1NnYTp3bnZHSkp1UUFLWWRGUDhCZmk0RzRhVTk2Q1lh" },
    }
}
) {
    var content = new StringContent("grant_type=password&username=Consumidor&password=curso");
    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
    var result = await client.PostAsync("https://localhost:9443/oauth2/token", content);
    Console.WriteLine($"StatusCode: {result.StatusCode}");
    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
}

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestHeaders = {
        { "Authorization", "Bearer eyJ4NXQiOiJNell4TW1Ga09HWXdNV0kwWldObU5EY3hOR1l3WW1NNFpUQTNNV0kyTkRBelpHUXpOR00wWkdSbE5qSmtPREZrWkRSaU9URmtNV0ZoTXpVMlpHVmxOZyIsImtpZCI6Ik16WXhNbUZrT0dZd01XSTBaV05tTkRjeE5HWXdZbU00WlRBM01XSTJOREF6WkdRek5HTTBaR1JsTmpKa09ERmtaRFJpT1RGa01XRmhNelUyWkdWbE5nX1JTMjU2IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJqYXZpZXIiLCJhdXQiOiJBUFBMSUNBVElPTiIsImF1ZCI6ImJ1dEtfYmJncVZveVpHMWNNZ2pOTVhKQ0pDd2EiLCJuYmYiOjE3MDk4MDAyOTQsImF6cCI6ImJ1dEtfYmJncVZveVpHMWNNZ2pOTVhKQ0pDd2EiLCJzY29wZSI6ImRlZmF1bHQiLCJpc3MiOiJodHRwczpcL1wvbG9jYWxob3N0Ojk0NDNcL29hdXRoMlwvdG9rZW4iLCJleHAiOjE3MDk4MDM4OTQsImlhdCI6MTcwOTgwMDI5NCwianRpIjoiMWQ2YmU4NTAtY2U3Zi00MmY2LWJkODUtMGMwN2RiMWFkYTVhIn0.h1mwECY6u-wTlmxbIFwfJUp9iTxLA7EE-qpv0jGQ43N0wfJJJlWb1kAtpiXnCSLB0Nv58Bd5zaVDoMvx2XrKAPMoFCG2OpuGBhxjtzYrQ_PQKfaXpBhOLAo5K7Zwme4eQMHekxkfYxgXn91ik2fv76_D_J96_Ej7kbq7AY9KibN71X8HTJ9UIdr1DPpyVdLVBzU9UP47bU5evvPgeGOcrQHoIyr20vatoMj4HVo6wUJuJ_5RCu4xbj7YkGuxM7RNta1bFGRv1inZ3rwvwKG8vYgyY3SCJEZ0fspAuyVJGDqxeF8O6EkuDMo5ClDj-PaICQzg14Gle0fEfnkEhFnGag" }
    }
}
) {
    var result = await client.GetAsync("https://localhost:8243/pizzashack/1.0.0/menu");
    Console.WriteLine($"StatusCode: {result.StatusCode}");
    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
}

//Console.ReadLine();


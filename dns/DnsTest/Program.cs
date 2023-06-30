// See https://aka.ms/new-console-template for more information
using System.Net;

Console.WriteLine("Hello, World!");
var dns=Dns.GetHostAddresses("api.adoptapet.com");
var client=new HttpClient();
var reponse = await client.GetAsync("https://restcountries.com/v3.1/all");
if (reponse.IsSuccessStatusCode)
{
    var str=await  reponse.Content.ReadAsStringAsync();
}
Console.WriteLine(dns.FirstOrDefault().ToString());

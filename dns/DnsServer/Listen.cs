using ARSoft.Tools.Net.Dns;
using System.Net;
using System.Net.Sockets;

namespace Dnsserver;

public class Listen
{
    void Listener()
    {
        using DnsServer server = new(new TcpServerTransport(IPAddress.Any, 10,10));
        server.QueryReceived += OnQueryReceived;

        server.Start();

        Console.WriteLine("Press any key to stop server");
        Console.ReadLine();
    }

    async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
    {
        if (e.Query is not DnsMessage query)
            return;

        var response = query.CreateResponseInstance();

        if ((query.Questions.Count == 1))
        {
            // send query to upstream server
            DnsQuestion question = query.Questions[0];
            DnsMessage? upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);

            // if got an answer, copy it to the message sent to the client
            if (upstreamResponse != null)
            {
                foreach (DnsRecordBase record in (upstreamResponse.AnswerRecords))
                {
                    response.AnswerRecords.Add(record);
                }
                foreach (DnsRecordBase record in (upstreamResponse.AdditionalRecords))
                {
                    response.AdditionalRecords.Add(record);
                }

                response.ReturnCode = ReturnCode.NoError;

                // set the response
                e.Response = response;
            }
        }
    }
}

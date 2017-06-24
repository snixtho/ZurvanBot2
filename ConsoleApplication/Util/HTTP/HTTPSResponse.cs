using System;
using System.Text;

namespace ZurvanBot.Util.HTTP
{
    public class HTTPSResponse
    {
        public HTTPSResponse(byte[] responseData)
        {
            ParseResponseData(responseData);
            Console.WriteLine(Encoding.ASCII.GetString(responseData));
        }

        private void ParseResponseData(byte[] responseData)
        {
            
        }
    }
}
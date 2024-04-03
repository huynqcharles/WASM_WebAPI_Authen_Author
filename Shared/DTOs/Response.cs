using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

        public Response()
        {
            
        }

        public Response(bool isSuccess, string token, string message)
        {
            IsSuccess = isSuccess;
            Token = token;
            Message = message;
        }
    }
}

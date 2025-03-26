using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjeApiModel.Enums;

namespace ProjeApiModel.DTOs.Response
{
    public class ResponseDTO<T> where T : class
    {
        public T? Data { get; set; }
        public StatusCode StatusCode { get; set; }
        public string Messages { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}

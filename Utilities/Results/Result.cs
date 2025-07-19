using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Results
{
    public class Result : IResult
    {
        public bool Success { get; }
        public string Message { get; }

        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }

        public Result(bool success)
        {
            Success = success;
        }

        public static Result Successful() => new Result(true);
        public static Result Successful(string message) => new Result(true, message);
        public static Result Failed() => new Result(false);
        public static Result Failed(string message) => new Result(false, message);
    }
}

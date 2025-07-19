using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T Data { get; }

        public DataResult(T data, bool success, string message) : base(success, message)
        {
            Data = data;
        }

        public DataResult(T data, bool success) : base(success)
        {
            Data = data;
        }

        public static DataResult<T> Successful(T data) => new DataResult<T>(data, true);
        public static DataResult<T> Successful(T data, string message) => new DataResult<T>(data, true, message);
        public static DataResult<T> Failed(T data) => new DataResult<T>(data, false);
        public static DataResult<T> Failed(T data, string message) => new DataResult<T>(data, false, message);
        public static DataResult<T> Failed(string message) => new DataResult<T>(default(T), false, message);
    }
}

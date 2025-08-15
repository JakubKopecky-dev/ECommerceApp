using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Common
{
    public readonly record struct Result<TValue, TError>(bool IsSuccess, TValue? Value, TError? Error)
    {
        public static Result<TValue, TError> Ok(TValue value) => new(true, value, default);
        public static Result<TValue, TError> Fail(TError error) => new(false, default, error);
    }
}

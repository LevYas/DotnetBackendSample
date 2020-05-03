using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SugarCounter.Core.Shared
{
    // There are two main classes to provide result of an operation:
    // 1. Res<TData, TError> - for operations which could return either some data or fail with error
    // 2. Res<TError> - for operations which could perform operation successfully or fail with error
    // 
    // Usage: just return data object (Res.Ok if there is no data) or enum with error code.
    //
    // They're built up using concepts of Monad and sum types in mind

    public abstract class ResBase<TError>
    {
        // Check for valid tuple collection with no duplicate keys
        // Intent is to hit possible error during the first debug run of Match methods
        [Conditional("DEBUG")]
        protected void CheckArgumentsInDebug<TResult>((TError, Func<TResult>)[] errorHandlers)
        {
            toDictionary(errorHandlers);
        }

        protected TResult MatchError<TResult>(TError errorValue, Func<TResult> defaultHandler,
            (TError, Func<TResult>)[] errorHandlers)
        {
            if (toDictionary(errorHandlers).TryGetValue(errorValue, out Func<TResult> handle))
                return handle();
            else
                return defaultHandler();
        }

        private Dictionary<TError, Func<TResult>> toDictionary<TResult>((TError, Func<TResult>)[] errorHandlers)
        {
            return errorHandlers.ToDictionary(k => k.Item1, e => e.Item2);
        }
    }

    public abstract class Res<TData, TError> : ResBase<TError>
    {
        public class Data : Res<TData, TError>
        {
            public Data(TData data) => Val = data;
            public TData Val { get; }
        }

        public static implicit operator Res<TData, TError>(TData value) => new Data(value);
        public TData GetDataOrDie() => this is Data data ? data.Val : throw new InvalidOperationException();

        // Err classes could not be merged into one because they have to be descendants of different classes
        public class Err : Res<TData, TError>
        {
            public Err(TError error) => Val = error;
            public TError Val { get; }
        }

        public static implicit operator Res<TData, TError>(TError error) => new Err(error);

        public TResult Match<TResult>(Func<TData, TResult> onOk, Func<TResult> defaultHandler,
            params (TError, Func<TResult>)[] errorHandlers)
        {
            CheckArgumentsInDebug(errorHandlers);

            if (this is Data data)
                return onOk(data.Val);

            return MatchError(((Err)this).Val, defaultHandler, errorHandlers);
        }

        public TResult Match<TResult>(Func<TData, TResult> onOk, Func<TError, TResult> onErr)
        {
            if (this is Data data)
                return onOk(data.Val);

            return onErr(((Err)this).Val);
        }

        public TResult Get<TResult>() where TResult : TData, TError
        {
            // Val's can't be null because they could be initialized only with values

            if (this is Data data)
                return (TResult)data.Val!;

            return (TResult)((Err)this).Val!;
        }

        public async Task<TResult> Match<TResult>(Func<TData, Task<TResult>> onOk, Func<TError, TResult> onErr)
        {
            if (this is Data data)
                return await onOk(data.Val);

            return onErr(((Err)this).Val);
        }

        public async Task<Res<TResult, TError>> Map<TResult>(Func<TData, Task<TResult>> convert)
        {
            if (this is Err err)
                return err.Val;

            return await convert(((Data)this).Val);
        }

        public Res<TResult, TError> Map<TResult>(Func<TData, TResult> convert)
        {
            if (this is Err err)
                return err.Val;

            return convert(((Data)this).Val);
        }

        public void Match(Action<TData> onOk, Action<TError> onError)
        {
            if (this is Data data)
                onOk(data.Val);
            else
                onError(((Err)this).Val);
        }
    }

    public abstract class Res<TError> : ResBase<TError>
    {
        public class NoData : Res<TError> { }
        public static implicit operator Res<TError>(Res.OkMarker _) => new NoData();

        // Err classes could not be merged into one because they have to be descendants of different classes
        public class Err : Res<TError>
        {
            public Err(TError error) => Val = error;
            public TError Val { get; }
        }

        public static implicit operator Res<TError>(TError error) => new Err(error);

        public TResult Match<TResult>(Func<TResult> onOk, Func<TResult> defaultHandler,
            params (TError, Func<TResult>)[] errorHandlers)
        {
            CheckArgumentsInDebug(errorHandlers);

            if (this is NoData)
                return onOk();

            return MatchError(((Err)this).Val, defaultHandler, errorHandlers);
        }
    }

    public static class Res
    {
        public class OkMarker { }
        public static OkMarker Ok { get; } = new OkMarker();

        public static async Task<Res<TResult, TError>> ThenMap<TResult, TData, TError>
            (this Task<Res<TData, TError>> inputTask, Func<TData, Task<TResult>> mapping)
        {
            return await (await inputTask).Map(input => mapping(input));
        }

        public static async Task<Res<TResult, TError>> ThenMap<TResult, TData, TError>
            (this Task<Res<TData, TError>> inputTask, Func<TData, TResult> mapping)
        {
            return (await inputTask).Map(input => mapping(input));
        }

        public static async Task<TResult> ThenMatch<TResult, TData, TError>
            (this Task<Res<TData, TError>> inputTask, Func<TData, TResult> onOk, Func<TError, TResult> onErr)
        {
            return (await inputTask).Match(onOk, onErr);
        }

        public static async Task<TOut> ThenGet<TOut>(this Task<Res<TOut, TOut>> inputTask)
        {
            return (await inputTask).Get<TOut>();
        }
    }
}
using System.Text.Json.Serialization.Metadata;

namespace ClubStat.Infrastructure
{
    internal static class MagicTypes
    {

        public static JsonTypeInfo<T> JsonTypeInfo<T>()
        {
            object result = typeof(T).Name switch
            {
                nameof(LogInResult) => LogInResultJsonContext.Default.LogInResult,
                nameof(Player)  =>  PlayerJsonContext.Default.Player,

                //need to update and add jsoncontext for missing type
                _ => throw new NotImplementedException()
            };

            return (JsonTypeInfo<T>)result; // Safe cast, since the cases handle the specific type checks
        }
    }
}

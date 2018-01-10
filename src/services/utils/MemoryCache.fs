namespace MyTeam

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Caching.Memory



module Cache =

    type Key = string

    type Query<'T> = Unit -> 'T

    type Get<'T> = HttpContext -> Key -> Query<'T>  -> 'T

    let getMemoryCache ctx = getService<IMemoryCache> ctx

    let get<'T> : Get<'T> =
        fun ctx key f ->
            let cache = getMemoryCache ctx
            let (success, result : 'T) = cache.TryGetValue(key)

            if not success then 
                let result = f()
                cache.Set(key, result) |> ignore
                result

            else
                result


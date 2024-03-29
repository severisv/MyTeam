namespace MyTeam

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Caching.Memory
open System
open Giraffe

module Cache =
    let options =
        let options = MemoryCacheEntryOptions()
        options.AbsoluteExpirationRelativeToNow <- Nullable <| TimeSpan.FromHours 1.0
        options
    
    type Key = string
    
    type Query<'T> = Unit -> 'T
    
    type Get<'T> = HttpContext -> Key -> Query<'T> -> 'T
    
    type Clear = HttpContext -> Key -> unit
    
    let getMemoryCache (ctx : HttpContext) = ctx.GetService<IMemoryCache>()
    
    let get<'T> : Get<'T> =
        fun ctx key query -> 
            let cache = getMemoryCache ctx
            let (success, result : 'T) = cache.TryGetValue(key)
            if not success then 
                let result = query()
                cache.Set(key, result, options) |> ignore
                result
            else result
    
    let clear : Clear =
        fun ctx key -> 
            let cache = getMemoryCache ctx
            cache.Remove key

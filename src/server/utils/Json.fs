namespace MyTeam

open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Thoth.Json.Net

module Json =
    open Shared

    let serialize obj =
        let settings = JsonSerializerSettings()
        settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()
        settings.Converters.Add(OptionConverter())
        settings.Converters.Add(IdiomaticDuConverter())
        settings.Converters.Add(StringEnumConverter())
        JsonConvert.SerializeObject(obj, settings)

    let fableSerialize obj = Encode.Auto.toString (obj)

    let fableDeserialize<'t> = Decode.Auto.fromString<'t>

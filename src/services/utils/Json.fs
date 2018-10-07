namespace MyTeam
open Newtonsoft.Json
open Newtonsoft.Json.Converters

module Json =
          
        let serialize obj =
            let settings = JsonSerializerSettings()
            settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()   
            settings.Converters.Add(OptionConverter())
            settings.Converters.Add(IdiomaticDuConverter())
            settings.Converters.Add(StringEnumConverter())
            JsonConvert.SerializeObject(obj, settings)

           


        let fableSerialize obj =
            let jsonConverter = Fable.JsonConverter() :> JsonConverter
            JsonConvert.SerializeObject(obj, [|jsonConverter|])
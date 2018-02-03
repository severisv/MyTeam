namespace MyTeam

open Giraffe.GiraffeViewEngine
open Giraffe

module AboutPages = 

    let index club user (ctx: HttpContext) =        
        [
            div [_class "mt-container"] [
                    img [_style "max-width: 100%"; _src "/images/wk2.jpg"]
                    br []
                    br [] 
                    p [] [encodedText "Wam-Kam FK er en norsk fotballklubb, stiftet 21. August 2007. Klubben har i dag to lag som spiller i henholdsvis 5.- og 7.-divisjon."]
                    p [] [encodedText "Wam-Kam er en forkortelse for Waldemarskameratene, og bakgrunnen for navnet er at klubben ble stiftet på Waldemars Café på St. Hanshaugen i Oslo, etter initiativ fra Øystein Hjelle Bondhus, Nicolay Storvand Dahl og Jørund Graadal Svestad."]
                    p [] [encodedText "Opprinnelig spilte Wam-Kam i svarte hjemmedrakter og oransje reservedrakter, men med tiden har flere i Wam-Kam-miljøet lagt sin elsk på de oransje draktene og foretrukket å bruke dem fremfor de svarte. Derfor spiller Wam-Kam nå de fleste kamper i oransje drakter, svarte shortser og oransje strømper."]
                    p [] [encodedText "Klubben har ingen egen hjemmebane, og spiller derfor sine hjemmekamper litt rundt omkring i Oslo."]
                ]
            div [] []
        ] 
        |> layout club user (fun o -> { o with Title = "Om klubben"}) ctx
        |> renderHtml
 
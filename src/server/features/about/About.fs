module MyTeam.About

open Fable.React
open Giraffe.ViewEngine
open Shared.Components
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Views
open Fable.React.Props
open MyTeam.Views.BaseComponents
open System.Linq


type About = string

type GetAbout = Database -> ClubId -> About

let getAbout: GetAbout =
    fun db clubId ->
        let (ClubId clubId) = clubId

        query {
            for club in db.Clubs do
                where (club.Id = clubId)
                select (club.About)
        }
        |> Seq.tryHead
        |> Option.defaultValue ""

let show (club: Club) (user: User option) (ctx: HttpContext) =
    let db = ctx.Database

    getAbout db club.Id
    |> fun about ->
        [ block [] [
              (user
               => (fun user ->
                   if user.IsInRole [ Role.Admin ] then
                       a [ _class "pull-right edit-link"
                           _href "/om/endre" ] [
                           !!(Icons.edit "Rediger side")
                       ]
                   else
                       empty))
              div [ _class "richtext" ] [
                  rawText about
              ]
          ] ]
        |> layout club user (fun o -> { o with Title = "Om klubben" }) ctx
        |> OkResult


let edit (club: Club) (user: User option) (ctx: HttpContext) =
    let db = ctx.Database

    getAbout db club.Id
    |> fun about ->
        [ block [] [
              form [ _action "/om/endre"; _method "POST" ] [
                  textarea [ _name "Value"
                             _class "form-control tinymce"
                             _placeholder "Innhold" ] [
                      rawText about
                  ]
                  br []
                  !!(btn [ Primary; Type "submit" ] [
                      Helpers.str "Lagre"
                     ])
              ]
          ] ]
        |> layout
            club
            user
            (fun o ->
                { o with
                    Title = "Om klubben"
                    Scripts = MyTeam.News.Pages.Components.tinyMceScripts })
            ctx
        |> OkResult


let editPost (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let form =
        ctx.BindForm<Shared.Components.Input.StringPayload>()

    db.Clubs.Where(fun c -> c.Id = clubId)
    |> Seq.tryHead
    |> function
        | Some c ->
            c.About <- form.Value
            db.SaveChanges() |> ignore
            Redirect "/om"
        | None -> NotFound



let privacy club user (ctx: HttpContext) =
    [ div [ _class "mt-container"
            _style "font-family: Arial;" ] [
        p [] [
            encodedText "Wamkam.no benytter seg av informasjonskapsler."
        ]
        br []
        p [] [
            encodedText "For ikke innloggede brukere lagrer wamkam.no kun anonymisert data om bruksmønster, vha. Google Analytics."
        ]
        br []
        p [] [
            encodedText
                "For medlemmer lagres kontaktinformasjon, samt statistikk fra kamp og trening. Kontaktinformasjonen brukes slik at medlemmer i klubben kan kontakte hverandre. Informasjon om deltakelse på kamp og trening brukes kun til å vise hvem som er med på hva."
        ]
        p [] [
            encodedText "Wam-Kam deler aldri persondata med tredjeparter."
        ]
        p [] [
            encodedText "For å fjerne all informasjon som er lagret om sin person, kontakt Severin Sverdvik på e-post: severin at sverdvik dot no."
        ]
        br []
        p [] [
            encodedText "Ved bruk av wamkam.no aksepterer man disse vilkårne."
        ]
      ]
      div [] [] ]
    |> layout club user (fun o -> { o with Title = "Personvern" }) ctx
    |> OkResult

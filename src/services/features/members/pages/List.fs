module MyTeam.Members.Pages.List


open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open MyTeam.Members
open MyTeam.Attendance
open MyTeam.Models.Enums


let view (club: Club) (user: Users.User) status next (ctx: HttpContext) =

    let status = match status with
                 | Some s -> Enums.tryParse<Status> s 
                             |> function 
                             | Ok s -> s
                             | Error _ -> Status.Aktiv
                 | None -> Status.Aktiv

    let members = Queries.listMembersDetailed ctx.Database club.Id

    let memberListUrl status = 
        sprintf "/intern/lagliste/%s" status

    let getImage = Images.getMember ctx

    ([
        main [] [
            block [] [
                tabs [] (
                    {
                        Items = [Status.Aktiv; Status.Veteran; Status.Inaktiv]
                                |> List.map (fun status -> 
                                                {
                                                    Text = status |> string
                                                    ShortText = status |> string
                                                    Url = memberListUrl (string status |> toLower)
                                                    Icon = Some <| playerStatusIcon status
                                                }
                                             )
                        IsSelected = (equals <| memberListUrl (status |> string |> toLower))
                    })
                br []
                table [] (
                            {
                                Columns = [
                                            col [Str ""] [NoSort; CellType Image; ClassName "hidden-xs"]
                                            col [Str "Navn"] []
                                            col [Str "Telefon"] []
                                            col [Str "E-post"] [ClassName "hidden-xs"]
                                            col [Str "Født"] [ClassName "visible-lg"]
                                ]
                                Rows = members
                                        |> List.filter (fun m -> m.Details.Status = status)
                                        |> List.map (fun m ->
                                                        [
                                                            Node (img [_src <| getImage m.Details.Image m.Details.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })]  )
                                                            Str m.Details.FullName
                                                            Node <| a [_href <| sprintf "tel:%s" m.Phone] [encodedText m.Phone]
                                                            Node <| a [_href <| sprintf "mailto:%s" m.Email] [encodedText m.Email]
                                                            Str (m.BirthYear |> toString)
                                                        ]
                                        )
                            }
                )
            ]
        ]
    ]
    |> layout club (Some user) id ctx
    |> htmlView) next ctx

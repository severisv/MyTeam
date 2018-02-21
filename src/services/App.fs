namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Views
open System
open System.Threading.Tasks

module AttendancePages = 

    type SelectedYear = 
    | AllYears
    | Year of int

    type PlayerStats = {
        Id: Guid
        FacebookId: string
        FirstName: string
        LastName: string
        UrlName: string
        Image: string
        Games: int
        Trainings: int
        NoShows: int
    }   
    with member p.Name = sprintf "%s %s" p.FirstName p.LastName    



    let index (club: Club) (user: Users.User) next (ctx: HttpContext) =

        let db = ctx.Database

       
        let getImage = Images.getMember ctx

        let attendancedance = []
        let years = []

        let isSelected str = true

        let statsUrl year =               
            let year = match year with
                       | AllYears _ -> "total"
                       | Year y -> str y           
            
            sprintf "/statistikk/%s" year       

        let registerAttendanceUrl = "oppmote/registrer"
      
        ([
            main [] [
                block [] [
                    (user.IsInRole [Role.Admin;Role.Trener;Role.Oppmøte] =? 
                       (div [_class "clearfix u-margin-bottom "] [ 
                            buttonLink registerAttendanceUrl Primary Sm [_class "pull-right hidden-lg hidden-md"] [
                                icon (fa "check-square-o") ""
                                encodedText " Registrer oppmøte"
                           ] 
                        ], 
                        emptyText))
                    table [] (
                        {
                            Type = Image
                            Columns = 
                                [
                                    { Value = [ Str "Spiller" ]; Align = Left }
                                    { Value = [ Node <| Icons.training ""; Node whitespace; Str "Trening" ]; Align = Center }
                                    { Value = [ Node <| Icons.game "" ; Node whitespace; Str "Kamp"]; Align = Center }
                                    { Value = [ Node <| icon (fa "warning") ""; Node whitespace; Str "Ikke møtt" ]; Align = Center }
                                ]
                            Rows = 
                                (attendancedance |> List.map (fun player ->
                                                    [
                                                        Node(span [] [
                                                                    img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] 
                                                                    whitespace 
                                                                    a [_href <| sprintf "/spillere/vis/%s" player.UrlName] 
                                                                        [
                                                                            encodedText player.Name
                                                                        ]                                                       
                                                                ] 
                                                                )                                    
                                                                                                                            
                                                        Number player.Trainings
                                                        Number player.Games
                                                        Number player.NoShows
                                                    ]
                                                    )
                                )
                        }
                    ) 
                ]
            ]          
            sidebar [] [
                (user.IsInRole [Role.Admin;Role.Trener;Role.Oppmøte] =? 
                   (block [] [ 
                        navList ({
                                    Header = "Adminmeny"
                                    Items = [{ Text = "Registrer oppmøte"; Url = registerAttendanceUrl}]
                                    IsSelected = fun _ -> false
                                    Footer = None
                                })
                       ] 
                    , emptyText))
                (years.Length > 0 =?
                    (block [] [
                        navList ({ 
                                    Header = "Sesonger"
                                    Items = years |> List.map (fun year  -> { Text = str year; Url = statsUrl (Year year) }                                                                   )  
                                    Footer = Some <| { Text = "Total"; Url = statsUrl AllYears }                                                               
                                    IsSelected = isSelected                                                               
                               })
                    ]
                   , emptyText)) 
            ]                                   
                    
        ] 
        |> layout club (Some user) (fun o -> { o with Title = "Oppmøte"}) ctx
        |> htmlView) next ctx


module App =
    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user


            match club with
                | Some club ->
                      choose [
                        route "/om" >-> (AboutPages.index club user)          
                        route "/statistikk" >=> StatsPages.index club user None None      
                        routef "/statistikk/%s/%s" (fun (teamName, year) -> StatsPages.index club user (Some teamName) (Some year))          
                        routef "/statistikk/%s" (fun teamName -> StatsPages.index club user (Some teamName) None)      
                        subRoute "/intern" 
                            (user |> Option.fold 
                                        (fun _ user ->
                                                (choose [ 
                                                    GET >=> choose [ 
                                                        route "/oppmote" >=> AttendancePages.index club user
                                                    ]                                
                                                ])
                                        )                        
                                        empty
                            )                           
                        route "/api/teams" >-> TeamApi.list club.Id
                        route "/api/players" >-> PlayerApi.list club.Id
                        route "/api/events" >=>                      
                            PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                                routef "/api/events/%s/description" (parseGuid >> EventApi.setDescription club.Id)
                        
                        subRoute "/api/members" 
                            (choose [ 
                                GET >=> choose [ 
                                    route "" >-> MemberApi.list club.Id
                                    route "/facebookids" >-> MemberApi.getFacebookIds club.Id
                                ]
                                PUT >=> 
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            routef "/%s/status" (parseGuid >> MemberApi.setStatus club.Id)
                                            routef "/%s/togglerole" (parseGuid >> MemberApi.toggleRole club.Id)
                                            routef "/%s/toggleteam" (parseGuid >> MemberApi.toggleTeam club.Id)
                                        ]       
                                POST >=>  
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            route "" >=> MemberApi.add club.Id
                                        ]
                            ])

                        subRoute "api/games"
                            (choose [
                                GET >=> 
                                    routef "/%s/squad" (parseGuid >> GameApi.getSquad club.Id)
                                                           
                                POST >=> 
                                    mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                        routef "/%s/score/home" (parseGuid >> GameApi.setHomeScore club.Id)
                                        routef "/%s/score/away" (parseGuid >> GameApi.setAwayScore club.Id)                                   
                                    ]
                                    mustBeInRole [Role.Trener] >=> choose [                                
                                        routef "/%s/gameplan" (parseGuid >> GameApi.setGamePlan club.Id)
                                        routef "/%s/gameplan/publish" (parseGuid >> GameApi.publishGamePlan club.Id)
                                    ]
                            ])                                                                                                                                                                                                                       
                       ] next ctx
                | None ->
                    choose [
                       ] next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp

    let addGiraffe (services : IServiceCollection)  =
        services.AddGiraffe() |> ignore



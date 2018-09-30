namespace MyTeam.Attendance.Pages

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open Attendance.Queries
open MyTeam.Attendance
open MyTeam.Shared.Components
open MyTeam.Components
open Fable.Helpers.React.Props
module Show =

    let view (club: Club) (user: Users.User) year (ctx: HttpContext) =

        let (selectedYear, years, attendance) =
            getAttendance ctx.Database club.Id year
       
        let getImage = Images.getMember ctx

        let attendanceUrl year =               
            let year = match year with
                       | AllYears _ -> "total"
                       | Year y -> string y           
            
            sprintf "/intern/oppmote/%s" year       

        let isSelected url = 
            attendanceUrl selectedYear = url      

        let registerAttendanceUrl = "/intern/oppmote/registrer"     
      
        [
            mtMain [] [
                block [] [
                    (user.IsInRole [Role.Admin;Role.Trener;Role.Oppmøte] =? 
                       (div [_class "clearfix u-margin-bottom "] [ 
                            !!(buttonLink registerAttendanceUrl Primary Sm [Class "pull-right hidden-lg hidden-md"] [
                                Icons.icon (fa "check-square-o") ""
                                Fable.Helpers.React.str " Registrer oppmøte"
                             ]) 
                        ], 
                        emptyText))
                    table [] 
                            [
                                col [CellType Image; NoSort] []
                                col [] [ encodedText "Spiller" ]
                                col [Align Center] [ !!(Icons.training "");span [_class "hidden-xs"][ encodedText " Trening" ]]
                                col [Align Center] [ !!(Icons.game "");span [_class "hidden-xs"][ encodedText " Kamp"]]
                                col [Align Center] [ icon (fa "warning") "";span [_class "hidden-xs"][ encodedText " Ikke møtt" ]]
                            ]                          
                            (attendance |> List.map (fun (player, attendance) ->
                                                let playerLink = a [_href <| sprintf "/spillere/vis/%s" player.UrlName; _title player.Name]
                                                tableRow [] [
                                                   playerLink [ img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] ]
                                                   playerLink [encodedText player.Name]                                                       
                                                   encodedText <| string attendance.Trainings
                                                   encodedText <| string attendance.Games
                                                   encodedText <| string attendance.NoShows
                                                ]
                                                )
                            )     
                ]
            ]          
            sidebar [] [
                (user.IsInRole [Role.Admin;Role.Trener;Role.Oppmøte] =? 
                   (block [] [ 
                        navList ({
                                    Header = "Adminmeny"
                                    Items = [{ Text = [icon (fa "check-square-o") "";encodedText " Registrer oppmøte"]; Url = registerAttendanceUrl}]
                                    IsSelected = fun _ -> false
                                    Footer = None
                                })
                       ] 
                    , emptyText))
                (years.Length > 0 =?
                    (block [] [
                        navList ({ 
                                    Header = "Sesonger"
                                    Items = years |> List.map (fun year  -> { Text = [encodedText <| string year]; Url = attendanceUrl (Year year) })  
                                    Footer = Some <| { Text = [encodedText "Total"]; Url = attendanceUrl AllYears }                                                               
                                    IsSelected = isSelected                                                               
                               })
                    ]
                   , emptyText)) 
            ]                                   
                    
        ] 
        |> layout club (Some user) (fun o -> { o with Title = "Oppmøte"}) ctx
        |> OkResult
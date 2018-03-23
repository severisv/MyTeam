namespace MyTeam.AttendancePages

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open Attendance.Queries
open MyTeam.Attendance

module Show =

    let view (club: Club) (user: Users.User) year next (ctx: HttpContext) =

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
                    table [] 
                            [
                                col [CellType Image] [ encodedText "Spiller" ]
                                col [TableAlignment Center] [ Icons.training ""; whitespace; encodedText "Trening" ]
                                col [TableAlignment Center] [ Icons.game "" ; whitespace; encodedText "Kamp"]
                                col [TableAlignment Center] [ icon (fa "warning") ""; whitespace; encodedText "Ikke møtt" ]
                            ]                          
                            (attendance |> List.map (fun (player, attendance) ->
                                                [
                                                    span [] [
                                                                img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] 
                                                                whitespace 
                                                                a [_href <| sprintf "/spillere/vis/%s" player.UrlName] 
                                                                    [
                                                                        encodedText player.Name
                                                                    ]                                                       
                                                            ]                                                           
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
        |> htmlView) next ctx
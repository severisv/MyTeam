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
                    table [] (
                        {
                            Columns = 
                                [
                                    { Value = [ Str "Spiller" ]; Props = [CellType Image] }
                                    { Value = [ Node <| Icons.training ""; Node whitespace; Str "Trening" ]; Props = [TableAlignment Center] }
                                    { Value = [ Node <| Icons.game "" ; Node whitespace; Str "Kamp"]; Props = [TableAlignment Center] }
                                    { Value = [ Node <| icon (fa "warning") ""; Node whitespace; Str "Ikke møtt" ]; Props = [TableAlignment Center] }
                                ]
                            Rows = 
                                (attendance |> List.map (fun (player, attendance) ->
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
                                                                                                                            
                                                        Number attendance.Trainings
                                                        Number attendance.Games
                                                        Number attendance.NoShows
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
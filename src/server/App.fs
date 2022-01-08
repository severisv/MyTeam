namespace Server

open MyTeam
open Shared
open Giraffe
open Server.Features
open Microsoft.AspNetCore.Hosting
open Shared.Features
open Shared.Domain.Members
open Server.Common
open Results
open Authorization
open PipelineHelpers

module App =

    let (webApp: HttpHandler) =
        removeTrailingSlash
        >=> fun next ctx ->
                let (club, user) = Tenant.get ctx
                let mustBeInRole = mustBeInRole user

                let urlIncludes (str: string) =
                    (Strings.toLower
                     >> ctx.Request.Path.ToString().Contains)
                        str

                match (club, user) with
                | (Some _, Some user) when
                    not user.ProfileIsConfirmed
                    && not <| urlIncludes "spillere/endre"
                    && not <| urlIncludes "api"
                    ->
                    redirectTo false $"/spillere/endre/%s{user.UrlName}" next ctx
                | (Some club, _) ->
                    choose
                        [ subRoute "/konto"
                          <| choose [ GET
                                      >=> route "/innlogging"
                                      >=> (Account.Login.view None [] club user |> htmlGet)
                                      POST
                                      >=> route "/innlogging"
                                      >=> Antiforgery.validate
                                      >=> (Account.Login.post club user |> htmlPost)
                                      POST
                                      >=> route "/utlogging"
                                      >=> Antiforgery.validate
                                      >=> (Account.Login.logOut club user |> htmlGet)
                                      POST
                                      >=> route "/innlogging/ekstern"
                                      >=> Antiforgery.validate
                                      >=> Account.Login.external
                                      GET
                                      >=> route "/innlogging/ekstern"
                                      >=> (Account.Login.externalCallback club user
                                           |> htmlGet)
                                      POST
                                      >=> route "/innlogging/ekstern/ny"
                                      >=> Antiforgery.validate
                                      >=> (Account.Login.signupExternal club user |> htmlPost)
                                      GET
                                      >=> route "/ny"
                                      >=> (Account.Signup.view None [] club user |> htmlGet)
                                      POST
                                      >=> route "/ny"
                                      >=> Antiforgery.validate
                                      >=> (Account.Signup.post club user |> htmlPost)
                                      GET
                                      >=> route "/glemt-passord"
                                      >=> (Account.ResetPassword.view None [] club user
                                           |> htmlGet)
                                      POST
                                      >=> route "/glemt-passord"
                                      >=> Antiforgery.validate
                                      >=> (Account.ResetPassword.post club user |> htmlPost)
                                      GET
                                      >=> route "/nullstill-passord"
                                      >=> (Account.ResetPassword.confirmView None [] club user
                                           |> htmlGet)
                                      POST
                                      >=> route "/nullstill-passord"
                                      >=> Antiforgery.validate
                                      >=> (Account.ResetPassword.confirmPost club user
                                           |> htmlPost)
                                      GET
                                      >=> (routef "/sletting/%s"
                                           <| fun userId ->
                                               (Account.RequestDeletion.showStatus club user userId
                                                |> htmlGet))
                                      POST
                                      >=> route "/sletting"
                                      >=> (Account.RequestDeletion.requestDeletion
                                           |> jsonPost) ]
                          route "/404"
                          >=> setStatusCode 404
                          >=> Views.Error.notFound club user
                          route "/personvern"
                          >=> (About.privacy club user |> htmlGet)
                          route "/"
                          >=> GET
                          >=> (News.Pages.Index.view club user id |> htmlGet)
                          routef "/%i/%i"
                          <| fun (skip, take) ->
                              GET
                              >=> (News.Pages.Index.view club user (fun o -> { o with Skip = skip; Take = take })
                                   |> htmlGet)
                          subRoute "/nyheter"
                          <| choose [ routef "/%i/%i"
                                      <| fun (skip, take) ->
                                          GET
                                          >=> (redirectTo true <| sprintf "/%i/%i" skip take)
                                      routef "/vis/%s"
                                      <| fun name ->
                                          GET
                                          >=> (News.Pages.Show.view club user name |> htmlGet)
                                      route "/vis"
                                      >=> (News.Pages.Show.redirectFromOldUrl club user)
                                      routef "/endre/%s"
                                      <| fun name ->
                                          mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Skribent ]
                                          >=> choose [ GET
                                                       >=> (user
                                                            => fun user -> (News.Pages.Edit.view club user name |> htmlGet))
                                                       POST
                                                       >=> (user
                                                            => fun user -> (News.Pages.Edit.editPost club user name |> htmlGet)) ]
                                      route "/ny"
                                      >=> mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Skribent ]
                                      >=> choose [ GET
                                                   >=> (user
                                                        => fun user -> (News.Pages.Edit.create club user |> htmlGet))
                                                   POST
                                                   >=> (user
                                                        => fun user -> (News.Pages.Edit.createPost club user |> htmlGet)) ]
                                      routef "/slett/%s"
                                      <| fun name ->
                                          mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Skribent ]
                                          >=> GET
                                          >=> (News.Pages.Edit.delete club name |> htmlGet) ]
                          subRoute "/kamper"
                          <| choose [ GET
                                      >=> choose [ route ""
                                                   >=> (Games.Pages.List.view club user None None
                                                        |> htmlGet)
                                                   routef "/%O"
                                                   <| fun gameId -> Games.Pages.Show.view club user gameId |> htmlGet
                                                   routef "/vis/%O" (fun (gameId: System.Guid) -> redirectTo true (sprintf "/kamper/%O" gameId))
                                                   routef "/%O/resultat"
                                                   <| fun gameId ->
                                                       mustBeInRole [ Role.Admin
                                                                      Role.Trener
                                                                      Role.Skribent ]
                                                       >=> (Games.Pages.Result.view club user gameId
                                                            |> htmlGet)
                                                   routef "/%O/laguttak"
                                                   <| fun gameId ->
                                                       mustBeInRole [ Role.Trener ]
                                                       >=> (Games.Pages.SelectSquad.view club user gameId
                                                            |> htmlGet)
                                                   routef "/%O/bytteplan"
                                                   <| fun gameId ->
                                                       user
                                                       => fun user ->
                                                           (Games.Pages.GamePlan.view club user gameId
                                                            |> htmlGet)
                                                   routef "/%s/%i"
                                                   <| fun (teamName, year) ->
                                                       Games.Pages.List.view club user (Some teamName) (Some year)
                                                       |> htmlGet
                                                   route "/ny"
                                                   >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                                   >=> (Games.Pages.Add.view club user |> htmlGet)
                                                   routef "/%O/endre" (fun gameId ->
                                                       mustBeInRole [ Role.Admin; Role.Trener ]
                                                       >=> (Games.Pages.Edit.view club user gameId |> htmlGet))
                                                   routef "/%s"
                                                   <| fun teamName ->
                                                       Games.Pages.List.view club user (Some teamName) None
                                                       |> htmlGet ] ]
                          subRoute "/spillere"
                          <| choose [ GET
                                      >=> choose [ route ""
                                                   >=> (Players.Pages.List.view club user "" |> htmlGet)
                                                   routef "/vis/%s" (Players.Pages.Show.view club user >> htmlGet)
                                                   routef "/endre/%s" (Players.Pages.Edit.view club user >> htmlGet)
                                                   routef "/%s" (Players.Pages.List.view club user >> htmlGet) ]

                                       ]
                          subRoute "/treninger"
                          <| choose [ GET
                                      >=> choose [ route "/ny"
                                                   >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                                   >=> (Trainings.Pages.Add.view club user |> htmlGet)
                                                   routef "/%O/endre" (fun trainingId ->
                                                       mustBeInRole [ Role.Admin; Role.Trener ]
                                                       >=> (Trainings.Pages.Edit.view club user trainingId
                                                            |> htmlGet)) ] ]
                          subRoute "/tabell"
                          <| choose [ GET
                                      >=> choose [ route ""
                                                   >=> (Table.Table.view club user None None |> htmlGet)
                                                   routef "/%s/%s"
                                                   <| fun (teamName, year) ->
                                                       Table.Table.view club user (Some teamName) (Some year)
                                                       |> htmlGet
                                                   routef "/%s"
                                                   <| fun teamName ->
                                                       Table.Table.view club user (Some teamName) None
                                                       |> htmlGet ] ]

                          subRoute "/statistikk"
                          <| choose [ GET
                                      >=> choose [ route ""
                                                   >=> (Stats.Pages.index club user None None |> htmlGet)
                                                   routef "/%s/%s"
                                                   <| fun (teamName, year) ->
                                                       Stats.Pages.index club user (Some teamName) (Some year)
                                                       |> htmlGet
                                                   routef "/%s"
                                                   <| fun teamName ->
                                                       Stats.Pages.index club user (Some teamName) None
                                                       |> htmlGet ] ]
                          route "/blimed"
                          >=> choose [ GET
                                       >=> (Members.Pages.RequestAccess.get club user
                                            |> htmlGet)
                                       POST
                                       >=> (Members.Pages.RequestAccess.post club user
                                            |> htmlPost) ]
                          route "/om"
                          >=> GET
                          >=> (About.show club user |> htmlGet)
                          route "/om/endre"
                          >=> mustBeInRole [ Role.Admin ]
                          >=> choose [ GET >=> (About.edit club user |> htmlGet)
                                       POST >=> (About.editPost club user |> htmlGet) ]
                          route "/støttespillere"
                          >=> GET
                          >=> (Sponsors.show club user |> htmlGet)
                          route "/støttespillere/endre"
                          >=> mustBeInRole [ Role.Admin ]
                          >=> choose [ GET >=> (Sponsors.edit club user |> htmlGet)
                                       POST >=> (Sponsors.editPost club user |> htmlGet) ]

                          subRoute "/intern" mustBeAuthenticated
                          >=> (user
                               => fun user ->
                                   choose [ GET
                                            >=> choose [ route ""
                                                         >=> (Events.List.upcoming club user |> htmlGet)
                                                         route "/arrangementer"
                                                         >=> redirectTo true "/intern"
                                                         route "/arrangementer/tidligere"
                                                         >=> (Events.List.previous club user None |> htmlGet)
                                                         routef
                                                             "/arrangementer/tidligere/%i"
                                                             ((fun year -> Events.List.previous club user (Some year))
                                                              >> htmlGet)
                                                         route "/arrangementer/varsler"
                                                         >=> (Views.NotificationViews.notificationPartial club user
                                                              |> htmlGet)
                                                         route "/lagliste"
                                                         >=> (Members.Pages.List.view club user None |> htmlGet)
                                                         routef "/lagliste/%s"
                                                         <| fun status ->
                                                             Members.Pages.List.view club user (Some status)
                                                             |> htmlGet
                                                         route "/oppmote/registrer"
                                                         >=> mustBeInRole [ Role.Admin
                                                                            Role.Trener
                                                                            Role.Oppmøte ]
                                                         >=> (Attendance.Pages.Register.view club user None
                                                              |> htmlGet)
                                                         routef "/oppmote/registrer/%O"
                                                         <| fun eventId ->
                                                             mustBeInRole [ Role.Admin
                                                                            Role.Trener
                                                                            Role.Oppmøte ]
                                                             >=> (Attendance.Pages.Register.view club user (Some eventId)
                                                                  |> htmlGet)
                                                         route "/oppmote"
                                                         >=> (Attendance.Pages.Show.view club user None
                                                              |> htmlGet)
                                                         routef "/oppmote/%s"
                                                         <| fun year ->
                                                             Attendance.Pages.Show.view club user (Some <| Strings.toLower year)
                                                             |> htmlGet
                                                         route "/boter/vis"
                                                         >=> (Fines.List.view club user None Fines.Common.AllMembers
                                                              |> htmlGet)
                                                         routef "/boter/vis/%s/%O"
                                                         <| fun (year, memberId) ->
                                                             Fines.List.view club user (year |> Some) (Fines.Common.Member memberId)
                                                             |> htmlGet
                                                         routef "/boter/vis/%s"
                                                         <| fun year ->
                                                             Fines.List.view club user (year |> Some) Fines.Common.AllMembers
                                                             |> htmlGet
                                                         route "/boter/innbetalinger"
                                                         >=> (Fines.Payments.view club user None Fines.Common.AllMembers
                                                              |> htmlGet)
                                                         routef "/boter/innbetalinger/%s/%O"
                                                         <| fun (year, memberId) ->
                                                             Fines.Payments.view club user (year |> Some) (Fines.Common.Member memberId)
                                                             |> htmlGet
                                                         routef "/boter/innbetalinger/%s"
                                                         <| fun year ->
                                                             Fines.Payments.view club user (year |> Some) Fines.Common.AllMembers
                                                             |> htmlGet
                                                         route "/boter/satser"
                                                         >=> (Fines.RemedyRates.view club user |> htmlGet)
                                                         route "/boter/oversikt"
                                                         >=> (Fines.Summary.view club user None |> htmlGet)
                                                         routef "/boter/oversikt/%s"
                                                         <| fun year ->
                                                             Fines.Summary.view club user (year |> Some)
                                                             |> htmlGet ] ])
                          route "/admin"
                          >=> mustBeAuthenticated
                          >=> GET
                          >=> mustBeInRole [ Role.Admin; Role.Trener ]
                          >=> (Admin.Pages.index club user |> htmlGet)
                          route "/admin/spillerinvitasjon"
                          >=> mustBeAuthenticated
                          >=> GET
                          >=> mustBeInRole [ Role.Admin; Role.Trener ]
                          >=> (Admin.Pages.invitePlayers club user |> htmlGet)

                          subRoute "/api/attendance"
                          <| choose [ POST
                                      >=> mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Oppmøte ]
                                      >=> routef
                                              "/%O/registrer/%O"
                                              (Attendance.Api.confirmAttendance club.Id
                                               >> jsonPost)
                                      POST
                                      >=> mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Oppmøte ]
                                      >=> routef "/%O/registrer/%O/victory" (Attendance.Api.registerVictory club.Id >> jsonPost) ]

                          route "/api/teams"
                          >=> (Teams.Api.list club.Id |> jsonGet)
                          subRoute
                              "/api/events"
                              (choose [ GET
                                        >=> route "/upcoming"
                                        >=> (user
                                             => fun user ->
                                                 Events.Api.listEvents club user (Client.Events.Upcoming Client.Events.Rest)
                                                 |> jsonGet)
                                        PUT
                                        >=> (user
                                             => fun user -> routef "/%O/signup" (Events.Api.signup club.Id user >> jsonPost))
                                        PUT
                                        >=> (user
                                             => fun user -> routef "/%O/signup/message" (Events.Api.signupMessage club.Id user >> jsonPost))
                                        PUT
                                        >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                        >=> routef "/%O/description" (Events.Api.setDescription club.Id >> jsonPost) ])
                          subRoute "/api/members"
                          <| choose [ GET
                                      >=> choose [ route "" >=> (Members.Api.list club.Id |> jsonGet)
                                                   route "/compact"
                                                   >=> (Members.Api.listCompact club.Id |> jsonGet) ]
                                      PUT
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> choose [ routef "/%O/status" (Members.Api.setStatus club.Id)
                                                   routef "/%O/togglerole" (Members.Api.toggleRole club.Id)
                                                   routef "/%O/toggleteam" (Members.Api.toggleTeam club.Id >> jsonPost) ]
                                      POST
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> choose [ route "" >=> (Members.Api.add club |> jsonPost) ] ]
                          subRoute "/api/games"
                          <| choose [ GET
                                      >=> choose [ routef "/insights/%s/%i" (Games.Api.getInsights club >> jsonGet)
                                                   routef "/%O/squad" (Games.Api.getSquad >> jsonGet)
                                                   route "/events/types"
                                                   >=> (Gameevents.getTypes |> jsonGet)
                                                   routef "/%O/events" (Gameevents.get club.Id >> jsonGet)
                                                   route "/refresh" >=> Games.Refresh.run

                                                    ]
                                      POST
                                      >=> mustBeInRole [ Role.Admin
                                                         Role.Trener
                                                         Role.Skribent ]
                                      >=> choose [ routef "/%O/score/home" (Games.Api.setHomeScore club.Id >> jsonPost)
                                                   routef "/%O/score/away" (Games.Api.setAwayScore club.Id >> jsonPost)
                                                   routef "/%O/events" (Gameevents.add club.Id >> jsonPost)
                                                   routef "/%O/events/%O/delete" (Gameevents.delete club.Id >> jsonGet)
                                                   routef "/%O/squad/select/%O" (Games.Api.selectPlayer club.Id >> jsonPost) ]
                                      mustBeInRole [ Role.Trener ]
                                      >=> choose [ routef "/%O/squad/publish" (Games.Api.publishSquad club.Id >> jsonPost)
                                                   routef "/%O/gameplan" (Games.Api.setGamePlan club.Id)
                                                   routef "/%O/gameplan/publish" (Games.Api.publishGamePlan club.Id >> jsonPost) ]
                                      mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> route ""
                                      >=> (Games.Api.add club |> jsonPost)
                                      PUT
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> routef "/%O" (Games.Api.update club >> jsonPost)
                                      DELETE
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> routef "/%O" (Games.Api.delete club >> jsonGet2)

                                       ]
                          subRoute "/api/trainings"
                          <| choose [ POST
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> route ""
                                      >=> (Trainings.Api.add club |> jsonPost)
                                      PUT
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> routef "/%O" (Trainings.Api.update club >> jsonPost)
                                      DELETE
                                      >=> mustBeInRole [ Role.Admin; Role.Trener ]
                                      >=> routef "/%O" (Trainings.Api.delete club >> jsonGet2)

                                       ]
                          subRoute "/api/players"
                          <| choose [ PUT
                                      >=> routef "/%O" (fun playerId ->
                                          ((authorizeUser
                                              (fun __ ->
                                                  match user with
                                                  | Some u when u.Id = playerId -> true
                                                  | Some u -> u.IsInRole [ Role.Admin; Role.Trener ]
                                                  | None -> false)
                                              accessDenied)
                                           >=> (Players.Api.update club playerId |> jsonPost)))


                                       ]
                          subRoute "/api/fines"
                          <| choose [ POST
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ route "" >=> (Fines.Api.add club |> jsonPost)

                                                    ]
                                      DELETE
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ routef "/%O" (Fines.Api.delete club >> jsonGet) ] ]
                          subRoute "/api/payments"
                          <| choose [ POST
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ route ""
                                                   >=> (Fines.Api.addPayment club |> jsonPost) ]
                                      PUT
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ route "/information"
                                                   >=> (Fines.Api.setPaymentInformation club |> jsonPost) ]
                                      DELETE
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ routef "/%O" (Fines.Api.deletePayment club >> jsonGet) ] ]
                          subRoute "/api/remedyrates"
                          <| choose [ GET
                                      >=> choose [ route ""
                                                   >=> (Fines.Api.listRemedyRates club
                                                        >> OkResult
                                                        |> jsonGet) ]
                                      POST
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ route ""
                                                   >=> (Fines.Api.addRemedyRate club |> jsonPost) ]
                                      PUT
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ route ""
                                                   >=> (Fines.Api.updateRemedyRate club |> jsonPost) ]
                                      DELETE
                                      >=> mustBeInRole [ Role.Botsjef ]
                                      >=> choose [ routef "/%O" (Fines.Api.deleteRemedyRate club >> jsonGet) ] ]
                          subRoute "/api/stats"
                          <| choose [ GET
                                      >=> choose [ routef "/player/%O" (Stats.Api.listAllPlayerStats club >> jsonGet) ] ]


                          subRoute "/api/tables"
                          <| choose [ GET
                                      >=> choose [ route "/refresh" >=> Table.Refresh.run ]
                                      mustBeInRole [ Role.Admin ]
                                      >=> choose [ PUT
                                                   >=> choose [ routef "/%s/%i/title" (Table.Api.setTitle club >> jsonPost)
                                                                routef "/%s/%i/fixturesourceurl" (Table.Api.setFixtureSourceUrl club >> jsonPost)
                                                                routef "/%s/%i/sourceurl" (Table.Api.setSourceUrl club >> jsonPost) ]
                                                   POST
                                                   >=> choose [ routef "/%s/%i/autoupdate" (Table.Api.setAutoUpdate club >> jsonPost)
                                                                routef "/%s/%i/autoupdatefixtures" (Table.Api.setAutoUpdateFixtures club >> jsonPost)
                                                                routef "/%s/%i" (Table.Api.create club >> jsonPost) ]
                                                   DELETE
                                                   >=> routef "/%s/%i" (Table.Api.delete club >> jsonGet) ]

                                       ]
                          setStatusCode 404
                          //    >=> ErrorHandling.logNotFound
                          >=> Views.Error.notFound club user ]
                        next
                        ctx
                | (None, _) ->
                    (setStatusCode 404
                     //  >=> ErrorHandling.logNotFound
                     >=> text "404")
                        next
                        ctx

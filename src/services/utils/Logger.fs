module MyTeam.Logger

open System
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

let get<'T> (serviceProvicer : IServiceProvider) = serviceProvicer.GetService<ILogger<'T>>()

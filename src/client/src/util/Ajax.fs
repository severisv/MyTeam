module Client.Util.Ajax

open Client.Util

let update elementId url =  
    Http.get
        url
        Ok
        { OnSuccess = fun r ->
                let container = Browser.Dom.document.getElementById elementId
                container.innerHTML <- r
          OnError = ignore }
                


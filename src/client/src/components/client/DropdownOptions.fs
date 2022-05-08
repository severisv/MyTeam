module Client.Components.DropdownOptions

open Shared.Util
open Feliz
open Fable.Core.JsInterop


let listenForOutsideClicks isListening setListening (ref: IRefValue<option<Browser.Types.HTMLElement>>) setIsOpen _ =

    setListening true





type Options =
    { Label: ReactElement
      OnClick: Browser.Types.MouseEvent -> Unit }

let dropDownOptions (options: Options list) (toggleButton: (Browser.Types.MouseEvent -> Unit) -> ReactElement) =

    let ref = React.useRef None

    let (isOpen, setIsOpen) = React.useState (false)
    let (isListening, setIsListening) = React.useState (false)

    let handleClickOutside (evt: Browser.Types.Event) =
        let (current: Browser.Types.HTMLElement) = ref.current.Value

        if current.contains (evt?target) |> not then
            setIsOpen false

    React.useEffect (
        (fun _ ->
            if not isListening && isOpen then
                setIsListening true

                [ "click"; "touchstart" ]
                |> List.iter (fun t -> Browser.Dom.document.addEventListener (t, handleClickOutside))

                React.createDisposable (fun _ ->
                    setIsListening false

                    [ "click"; "touchstart" ]
                    |> List.iter (fun t -> Browser.Dom.document.removeEventListener (t, handleClickOutside)))
            else
                React.createDisposable (fun _ -> ())),
        [| box isOpen |]
    )



    Html.div [
        prop.ref ref
        prop.style [ style.position.relative ]
        prop.children [
            (toggleButton (fun _ -> setIsOpen (not isOpen)))
            Html.div [
                prop.style [
                    if isOpen then
                        style.display.block
                    else
                        style.display.none
                ]
                prop.className "dropdown-options"
                prop.children (
                    options
                    |> List.map (fun o ->
                        Html.button [
                            prop.onClick (fun e ->
                                setIsOpen false
                                o.OnClick e)
                            prop.children [ o.Label ]
                        ])
                )
            ]
        ]
    ]

module Client.Components.DropdownOptions

open Shared.Util
open Feliz


type Options =
    { Label: ReactElement
      OnClick: Browser.Types.MouseEvent -> Unit }

let dropDownOptions (options: Options list) (toggleButton: (Browser.Types.MouseEvent -> Unit) -> ReactElement) =
    let (isOpen, setIsOpen) = React.useState (false)

    Html.div [
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

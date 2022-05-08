module List

let toggle element list =
    if list |> List.contains element then
        list |> List.filter ((<>) element)
    else
        list @ [ element ]


let mapWhen filterFn mappingFn list =
    list
    |> List.map (fun element ->
        if filterFn element then
            mappingFn element
        else
            element)

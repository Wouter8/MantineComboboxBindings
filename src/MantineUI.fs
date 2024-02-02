namespace MantineUI

open System.ComponentModel
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz
open Fable



[<AutoOpen ; EditorBrowsable(EditorBrowsableState.Never)>]
module ManHelpers =
    [<Emit("$0 === undefined")>]
    let private isUndefined x = jsNative
    let fromFlatEntries (kvs: seq<string * obj>) : obj =
        let rec setProperty (target: obj) (key: string) (value: obj) =
            match key.IndexOf '.' with
            | -1 -> target?(key) <- value
            | sepIdx ->
                let topKey = key.Substring (0, sepIdx)
                let nestedKey = key.Substring (sepIdx + 1)

                if isUndefined target?(topKey) then
                    target?(topKey) <- obj ()

                setProperty target?(topKey) nestedKey value

        let target = obj ()

        for (key, value) in kvs do
            setProperty target key value

        target

    let inline reactElement (el: ReactElementType) (props: 'a) : ReactElement = import "createElement" "react"

    let inline createElement (el: ReactElementType) (props: IReactProperty list) : ReactElement =
        reactElement el (!!props |> fromFlatEntries)

[<Global>]
type UseComboboxOptions
    [<ParamObject; Emit("$0")>]
    (
        ?onDropdownClose: (unit -> unit)
    ) =
    member val onDropdownClose : (unit -> unit) option = jsNative with get, set

type ComboboxStore =
    abstract openDropdown: (unit -> unit) with get, set
    abstract closeDropdown: (unit -> unit) with get, set
    abstract toggleDropdown: (unit -> unit) with get, set
    abstract resetSelectedOption: (unit -> unit) with get, set


type Man =
    [<Hook>]
    static member inline useCombobox (options: UseComboboxOptions) : ComboboxStore = import "useCombobox" "@mantine/core"
    static member inline combobox props = createElement (import "Combobox" "@mantine/core") props
    static member inline comboboxTarget props = createElement (import "ComboboxTarget" "@mantine/core") props
    static member inline comboboxDropdown props = createElement (import "ComboboxDropdown" "@mantine/core") props
    static member inline comboboxOptions props = createElement (import "ComboboxOptions" "@mantine/core") props
    static member inline comboboxOption props = createElement (import "ComboboxOption" "@mantine/core") props
    static member inline comboboxChevron props = createElement (import "ComboboxChevron" "@mantine/core") props
    static member inline mantineProvider props = createElement (import "MantineProvider" "@mantine/core") props
    static member inline inputBase props = createElement (import "InputBase" "@mantine/core") props
    static member inline inputPlaceholder props = createElement (import "InputPlaceholder" "@mantine/core") props

type combobox =
    static member inline store (value: ComboboxStore) = Interop.mkAttr "store" value
    static member inline onOptionSubmit (value: (string -> unit)) = Interop.mkAttr "onOptionSubmit" value

type comboboxTarget =
    static member inline children (value: ReactElement) = Interop.mkAttr "children" value

type comboboxOption =
    static member inline value (value: string) = Interop.mkAttr "value" value

type inputBase =
    static member inline component' (value: string) = Interop.mkAttr "component" value
    static member inline type' (value: string) = Interop.mkAttr "type" value
    static member inline pointer = Interop.mkAttr "pointer" true
    static member inline rightSection (value: ReactElement) = Interop.mkAttr "rightSection" value
    static member inline rightSectionPointerEvents (value: string) = Interop.mkAttr "rightSectionPointerEvents" value

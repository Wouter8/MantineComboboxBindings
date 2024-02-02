module Test

open Elmish
open Fable.Core
open Feliz
open Feliz.Router
open Feliz.UseElmish
open MantineUI

JsInterop.importSideEffects "@mantine/core/styles.layer.css"

type State = {
    Value: Option<string>
}

type Msg =
    | ValueSelected of string
    | NoOp

let private init () =
    { Value = None }, Cmd.none

let private update (comboboxStoreRef: IRefValue<ComboboxStore>) msg state =
    let comboboxStore = comboboxStoreRef.current
    match msg with
    | ValueSelected value ->
        comboboxStore.closeDropdown ()
        { state with Value = Some value }, Cmd.none
    | NoOp ->
        JS.console.log (comboboxStore)
        state, Cmd.none

[<ReactComponent>]
let Page() =
    // Create a ref which initially has no value, but has the correct type.
    let comboboxStoreRef = React.useRef Unchecked.defaultof<ComboboxStore>
    // Initialize the combobox store. We can use the ref in the callback functions :)
    let comboboxStore = Man.useCombobox (new UseComboboxOptions (onDropdownClose = fun () -> comboboxStoreRef.current.resetSelectedOption ()))
    // Set the combobox store in the ref.
    comboboxStoreRef.current <- comboboxStore

    // Use the ref in the update function. `ref.current` does correctly get updated.
    let updateFunc = update comboboxStoreRef
    let state, dispatch = React.useElmish (init, updateFunc)

    React.useEffectOnce (fun _ ->
        // This now prints the updated combobox store every 2 seconds :)
        JS.setInterval (fun _ -> dispatch NoOp) 2000 |> ignore
    )

    Html.div [
        prop.style [ style.padding 32 ]
        prop.children [
            Man.mantineProvider [
                prop.children [
                    Man.combobox [
                        combobox.store comboboxStore
                        combobox.onOptionSubmit (ValueSelected >> dispatch)

                        prop.children [
                            Man.comboboxTarget [
                                comboboxTarget.children <|
                                    Man.inputBase [
                                        inputBase.component' "button"
                                        inputBase.type' "button"
                                        inputBase.pointer
                                        inputBase.rightSection (Man.comboboxChevron [])

                                        prop.onClick (fun _ -> comboboxStore.toggleDropdown ())

                                        prop.children [
                                            match state.Value with
                                            | Some v -> Html.text v
                                            | None -> Man.inputPlaceholder [ prop.children [ Html.text "Pick value" ] ]
                                        ]
                                    ]
                                ]

                            Man.comboboxDropdown [
                                prop.children [
                                    Man.comboboxOptions [
                                        prop.children [
                                            Man.comboboxOption [ comboboxOption.value "Option 1" ; prop.children [ Html.text "Option 1" ] ]
                                            Man.comboboxOption [ comboboxOption.value "Option 2" ; prop.children [ Html.text "Option 2" ] ]
                                            Man.comboboxOption [ comboboxOption.value "Option 3" ; prop.children [ Html.text "Option 3" ] ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

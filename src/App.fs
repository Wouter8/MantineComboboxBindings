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

// [Issue 1] Pass combobox to update function such that...
let private update (comboboxStore: ComboboxStore) msg state =
    match msg with
    | ValueSelected value ->
        // [Issue 1] ... I'm able to call functions of ComboboxStore from here
        comboboxStore.closeDropdown ()
        { state with Value = Some value }, Cmd.none
    | NoOp ->
        // [Issue 2] The comboboxStore parameter is not updated
        JS.console.log (comboboxStore)
        state, Cmd.none

[<ReactComponent>]
let Page() =
    // [Issue 1]
    // The example contains the following code:
    //      const combobox = useCombobox({
    //          onDropdownClose: () => combobox.resetSelectedOption(),
    //      });
    // In F# it's not possible to call combobox.resetSelectedOption() at the same place.
    // let comboboxStore = Man.useCombobox (new UseComboboxOptions (onDropdownClose = fun () -> comboboxStore.resetSelectedOption ()))
    // let state, dispatch = React.useElmish (init, update comboboxStore)

    // Therefore, it probably has to be controlled, but then I have to dispatch messages, so I cannot pass comboboxStore to the update function.
    // let state, dispatch = React.useElmish (init, update)
    // let comboboxStore = Man.useCombobox (new UseComboboxOptions (onDropdownClose = fun () -> dispatch (SetSelectedOption None)))

    // I prefer not to have to control everything about the combobox, because that can get out of hand quite quickly.
    // How to solve this?
    let comboboxStore = Man.useCombobox (new UseComboboxOptions ())

    // [Issue 2] I tried to pass the current comboboxStore to the update function like this
    let updateFunc = update comboboxStore
    let state, dispatch = React.useElmish (init, updateFunc)

    React.useEffectOnce (fun _ ->
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

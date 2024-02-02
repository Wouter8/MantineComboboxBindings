module Main

open Feliz
open Browser.Dom

let root = ReactDOM.createRoot(document.getElementById "feliz-app")
root.render(Test.Page())

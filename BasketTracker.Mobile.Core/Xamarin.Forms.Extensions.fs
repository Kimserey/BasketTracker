namespace BasketTracker.Mobile.Core

open System
open System.ComponentModel
open Xamarin.Forms

[<AutoOpen>]
module Extensions =

    type Label with
        static member SetBinding' prop (name: string) (label: Label) =
            label.SetBinding(prop, name)
            label


    type ListView with
        static member SetTemplateBinding prop (name: string) (list: ListView) =
            list.ItemTemplate.SetBinding(prop, name)
            list
        
        static member IsEnabled' isEnabled (list:ListView) =
            list.IsEnabled <- isEnabled
            list

        static member SetBinding' prop (name: string) (list: ListView) =
            list.SetBinding(prop, name)
            list


    type StackLayout with
        static member AddChild child (layout: StackLayout) =
            layout.Children.Add child
            layout

    
    type MenuItem with
        static member SetBinding' prop (name: string) (menu: MenuItem) =
            menu.SetBinding(prop, name)
            menu
        

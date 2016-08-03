namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Domain
open SQLite.Net

type AddStorePage(factory: SQLiteConnectionFactory) as self =
    inherit ContentPage(Title = "Add new store")
    
    let nameEntry =
        new Entry(Placeholder = "Enter a store name here")

    let save =
        new ToolbarItem(
            "Save this store", 
            "save", 
            fun () -> 
                factory.ExecuteQuery
                    (fun conn ->
                        Storage.Store.Add conn nameEntry.Text
                        nameEntry.Text <- "")

                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        new StackLayout()
        |> StackLayout.AddChild nameEntry

    do
        base.ToolbarItems.Add(save)
        base.Content <- layout

type StoresPage(factory: SQLiteConnectionFactory) as self =
    inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit "shop")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<TextCell>))
        |> ListView.SetTemplateBinding TextCell.TextProperty "Name"
    
    let add =
        let page =
            new AddStorePage(factory)

        new ToolbarItem(
            "Add new store", 
            "plus", 
            fun () -> 
                self.Navigation.PushAsync(page)
                |> Async.AwaitTask
                |> Async.StartImmediate)
    
    let refresh() =
        factory.ExecuteQuery 
            (fun conn ->
                listView.ItemsSource <- Storage.Store.Get conn)
    
    do
        base.ToolbarItems.Add(add)
        base.Content <- listView

    override self.OnAppearing() =
        refresh()

type App() = 
    inherit Application()

    do 
        base.MainPage <- new NavigationPage(new StoresPage(Storage.factory))

﻿namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Domain
open SQLite.Net

type StoreViewModel(store: Storage.Store, connectionFactory: SQLiteConnectionFactory, host: Page) =
    inherit ViewModelBase()

    let mutable name = store.Name
    let mutable host = host
    
    member self.ConnectionFactory =
        connectionFactory

    member self.Host
        with get()    = host
        and set value = host <- value

    member self.Name
        with get () = name
        and set value =
            self.OnPropertyChanging("Name") 
            name <- value
            self.OnPropertyChanged("Name")

    member self.EditName() =
        use conn = connectionFactory() 
        Storage.Store.Update store name conn

    member self.Archive() =
        use conn = connectionFactory() 
        Storage.Store.Archive store conn

    static member ToViewModel connectionFactory host store =
        new StoreViewModel(store, connectionFactory, host)
    
type StorePage() as self =
    inherit ContentPage()
    
    do
        base.SetBinding(ContentPage.TitleProperty, "Name")
    
type EditStorePage(vm: StoreViewModel) as self =
    inherit ContentPage(Title = "Edit existing store")

    let nameEntry =
        let entry = new Entry()
        entry.SetBinding(Entry.TextProperty, "Name")
        entry.Completed.Add(fun _ -> 
            vm.Name <- entry.Text
        )
        entry

    let save =
        new ToolbarItem(
            "Save your changes", 
            "save", 
            fun () -> 
                vm.EditName()
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    do
        self.BindingContext <- vm
        self.ToolbarItems.Add(save)
        self.Content <- new StackLayout() |> StackLayout.AddChild nameEntry


type AddStorePage(connectionFactory: SQLiteConnectionFactory) as self =
    inherit ContentPage(Title = "Add new store")
    
    let nameEntry =
        new Entry(Placeholder = "Enter a store name here")

    let save =
        new ToolbarItem(
            "Save this store", 
            "save", 
            fun () -> 
                use conn = connectionFactory()
                Storage.Store.Add conn nameEntry.Text
                nameEntry.Text <- ""

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

type StoreViewCell() as self =
    inherit ViewCell()

    let edit =
        new MenuItem(
            Text = "Edit",  
            Icon = FileImageSource.op_Implicit "pencil")

    let delete =
        new MenuItem(
            Text = "Delete", 
            Icon = FileImageSource.op_Implicit "bin")

    let layout =
        new StackLayout()
        |> StackLayout.AddChild (new Label() |> Label.SetBinding' Label.TextProperty "Name")

    do
        edit.Clicked.Add(fun e ->
            let vm = self.BindingContext :?> StoreViewModel
            let page = new EditStorePage(vm)
            let host = vm.Host :?> StoresPage
            host.Navigation.PushAsync(page)
            |> Async.AwaitTask
            |> Async.StartImmediate
        )

        delete.Clicked.Add(fun e ->
            let vm = self.BindingContext :?> StoreViewModel
            vm.Archive()
            let host = vm.Host :?> StoresPage
            host.Refresh()
        )

        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)
        
        self.View <- layout

and StoresPage(connectionFactory: SQLiteConnectionFactory) as self =
    inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit "shop")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<StoreViewCell>))
    
    let add =
        let page =
            new AddStorePage(connectionFactory)

        new ToolbarItem(
            "Add new store", 
            "plus", 
            fun () -> 
                self.Navigation.PushAsync(page)
                |> Async.AwaitTask
                |> Async.StartImmediate)
    
    do
        self.ToolbarItems.Add(add)
        self.Content <- listView
        
        listView.ItemTapped.Add(fun e ->
            let page = new StorePage()
            let vm = e.Item :?> StoreViewModel
            vm.Host <- page
            page.BindingContext <- vm

            self.Navigation.PushAsync(page)
            |> Async.AwaitTask
            |> Async.StartImmediate
            listView.SelectedItem <- null)
            
    member self.Refresh() =
        use conn = connectionFactory() 
        listView.ItemsSource <- 
            Storage.Store.List conn 
            |> List.map (StoreViewModel.ToViewModel connectionFactory self)

    override self.OnAppearing() =
        self.Refresh()

type App() = 
    inherit Application()

    do 
        base.MainPage <- new NavigationPage(new StoresPage(Storage.connectionFactory))

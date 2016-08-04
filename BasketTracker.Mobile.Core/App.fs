﻿namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Model
open SQLite.Net
//
//type StoreViewModel(store: Storage.Store, connectionFactory: SQLiteConnectionFactory, host: Page) =
//    inherit ViewModelBase()
//
//    let mutable name = store.Name
//    let mutable host = host
//    
//    member self.ConnectionFactory =
//        connectionFactory
//
//    member self.Host
//        with get()    = host
//        and set value = host <- value
//
//    member self.Name
//        with get () = name
//        and set value =
//            self.OnPropertyChanging("Name") 
//            name <- value
//            self.OnPropertyChanged("Name")
//
//    member self.EditName() =
//        use conn = connectionFactory() 
//        Storage.SQLStore.Update store.Id name conn
//
//    member self.Archive() =
//        use conn = connectionFactory() 
//        Storage.SQLStore.Archive store.Id conn
//
//    static member ToViewModel connectionFactory host store =
//        new StoreViewModel(store, connectionFactory, host)
//    
//type StorePage() as self =
//    inherit ContentPage()
//    
//    do
//        base.SetBinding(ContentPage.TitleProperty, "Name")

type UpdateStorePage() as self =
    inherit ContentPage()

    let entry =
        new Entry(Placeholder = "Enter a store name here")

    let save =
        new ToolbarItem(
            "Save your changes", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    do
        self.SetBinding(ContentPage.TitleProperty, "Title")
        entry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "UpdateCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

        self.ToolbarItems.Add(save)
        self.Content <- new StackLayout() |> StackLayout.AddChild entry


type AddStorePage(vm: AddStoreViewModel) as self =
    inherit ContentPage()
    
    let entry =
        new Entry(Placeholder = "Enter a store name here")

    let save =
        new ToolbarItem(
            "Save this store", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    do
        self.BindingContext <- vm

        self.SetBinding(ContentPage.TitleProperty, "Title")
        entry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

        base.ToolbarItems.Add(save)
        base.Content <- new StackLayout() |> StackLayout.AddChild entry

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
            let host = self.Context.Host :?> StoreListPage
            let page = new UpdateStorePage()
            page.BindingContext <- self.Context.GetUpdateViewModel()

            host.Navigation.PushAsync(page)
            |> Async.AwaitTask
            |> Async.StartImmediate
        )

        delete.SetBinding(MenuItem.CommandProperty, "ArchiveCommand")
        delete.Clicked.Add(fun e ->
            let vm = self.BindingContext :?> StoreCellViewModel
            let host = vm.Host :?> StoreListPage
            host.Refresh()
        )

        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)

        self.View <- layout

    member self.Context
        with get(): StoreCellViewModel =
            unbox<StoreCellViewModel> self.BindingContext

and StoreListPage(vm: StoreListViewModel) as self =
    inherit ContentPage(Title = "Stores")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<StoreViewCell>))
    
    let add =
        new ToolbarItem(
            "Add new store", 
            "plus", 
            fun () ->
                let addPage = new AddStorePage(self.Context.NavigateToAddStoreViewModel())
                self.Navigation.PushAsync(addPage)
                |> Async.AwaitTask
                |> Async.StartImmediate)
    
    do
        self.BindingContext <- vm

        self.ToolbarItems.Add(add)
        self.Content <- listView
//        listView.ItemTapped.Add(fun e ->
//            let page = new StorePage()
//            let vm = e.Item :?> StoreViewModel
//            vm.Host <- page
//            page.BindingContext <- vm
//
//            self.Navigation.PushAsync(page)
//            |> Async.AwaitTask
//            |> Async.StartImmediate
//            listView.SelectedItem <- null)
            
    member self.Refresh() =
       listView.ItemsSource <- (self.Context.List |> List.map (fun x -> x.Host <- self; x))

    override self.OnAppearing() =
        self.Refresh()

    member self.Context
        with get(): StoreListViewModel =
            unbox<StoreListViewModel> self.BindingContext


type App() = 
    inherit Application()

    do 
        base.MainPage <- new NavigationPage(new StoreListPage(new StoreListViewModel()))

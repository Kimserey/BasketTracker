﻿namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Model
open SQLite.Net

type BasketListPage(vm: BasketListViewModel) as self =
    inherit ContentPage()
    
    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<TextCell>))
    
    let label = 
        new Label()

    let layout = 
        let layout = new StackLayout()
        layout.Children.Add(label)
        layout.Children.Add(listView)
        layout

    do
        self.BindingContext <- vm

        self.SetBinding(ContentPage.TitleProperty, "Title")
        label.SetBinding(Label.TextProperty, "Total")
        listView.SetBinding(ListView.ItemsSourceProperty, "List")

        self.Content <- layout


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
            let page = new UpdateStorePage()
            page.BindingContext <- self.Context.GetUpdateViewModel()

            self.ParentView.Navigation.PushAsync(page)
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
    
    override self.OnTapped() =
        self.ParentView.Navigation.PushAsync(
            new BasketListPage(
                self.Context.NavigateToBaskListViewModel()
            )
        )
        |> Async.AwaitTask
        |> Async.StartImmediate

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

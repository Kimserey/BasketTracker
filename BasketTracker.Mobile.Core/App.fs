﻿namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open SQLite.Net
open Models

type BasketViewCell() =
    inherit ViewCell()

    let layout  = new StackLayout(Orientation = StackOrientation.Horizontal)
    let image   = new Image(Source = FileImageSource.op_Implicit "basket")
    let date    = new Label()
    let amount  = new Label()

    do
        date.SetBinding(Label.TextProperty, "Date", stringFormat = "dd MMM YYYY")
        amount.SetBinding(Label.TextProperty, "Amount", stringFormat = "C")
        layout.Children.Add(image)
        layout.Children.Add(date)
        layout.Children.Add(amount)

type BasketListPage(getBasketList, goToItemList) as self =
    inherit ContentPage()
    
    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<BasketViewCell>))
    
    let label = 
        new Label()

    let layout = 
        let layout = new StackLayout()
        layout.Children.Add(label)
        layout.Children.Add(listView)
        layout

    do
        listView.ItemsSource <- getBasketList()
        self.SetBinding(ContentPage.TitleProperty, "Title")
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


type AddStorePage() as self =
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
        self.SetBinding(ContentPage.TitleProperty, "Title")
        entry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")


        base.ToolbarItems.Add(save)
        base.Content <- new StackLayout() |> StackLayout.AddChild entry


type StoreCell = {
    Id: int
    Name: string 
    GoToEdit: INavigation -> StoreCell -> unit
    RefreshStoreList: unit -> unit
}

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
        edit.Clicked.Add(fun e -> self.Context.GoToEdit self.ParentView.Navigation self.Context)
        
        delete.SetBinding(MenuItem.CommandProperty, "ArchiveCommand")
        delete.Clicked.Add(fun _ -> self.Context.RefreshStoreList())

        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)

        self.View <- layout

    member self.Context
        with get(): StoreCell =
            unbox<StoreCell> self.BindingContext


and StoreListNavigator = {
    GoToAddPage: INavigation -> unit
    GoToEditPage: INavigation -> unit
    GoToBasketList: INavigation -> unit
}

and StoreListPage<'TStore>(getStoreList, goToAdd, goToEdit, goToBaskets) as self =
    inherit ContentPage(Title = "Stores")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<StoreViewCell>))
    
    let add =
        new ToolbarItem(
            "Add new store", 
            "plus", 
            fun () -> goToAdd self.Navigation)
    
    do
        self.ToolbarItems.Add(add)
        self.Content <- listView
            
        listView.ItemTapped.Add(fun e -> goToBaskets self.Navigation <| unbox<'TStore> e.Item)
            
    member self.Refresh() =
        listView.ItemsSource <- 
            getStoreList() 
            |> List.map(fun (s: Store) -> 
                { Id = s.Id
                  Name = s.Name
                  GoToEdit = goToEdit
                  RefreshStoreList = self.Refresh })

    override self.OnAppearing() =
        self.Refresh()
        
type App() = 
    inherit Application()


    let page = 
        let addPage             = new AddStorePage()
        let updateStorePage     = new UpdateStorePage()
        
        let basketListPage = 
            new BasketListPage(
                getBasketList = Baskets.list,
                goToItemList =
                    (fun nav -> ())
            )
        
        new StoreListPage<StoreCell>(
            getStoreList = 
                Stores.list,
            goToAdd = 
                (fun nav ->
                    addPage.BindingContext <- new AddStoreViewModel(Stores.add)
                    nav.PushAsync(addPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToEdit =
                (fun nav store ->
                    updateStorePage.BindingContext <- new UpdateStoreViewModel(store.Name, Stores.update store.Id)
                    nav.PushAsync(updateStorePage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToBaskets = 
                (fun nav store ->
                    basketListPage.BindingContext <- new PageViewModel(Title = store.Name)
                    nav.PushAsync(basketListPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate)
        )

    do 
        page.BindingContext <- new PageViewModel(Title = "Stores")
        base.MainPage <- new NavigationPage(page)
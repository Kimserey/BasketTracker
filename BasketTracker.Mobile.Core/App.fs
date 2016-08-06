namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open SQLite.Net
open Models

type BasketViewCell() as self=
    inherit ViewCell()

    let grid    = new Grid()
    let image   = new Image(Source = FileImageSource.op_Implicit "basket")
    let date    = new Label(YAlign = TextAlignment.Center)
    let amount  = new Label(YAlign = TextAlignment.Center)

    do
        date.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:dd MMM yyyy - hh:mm tt}")
        amount.SetBinding(Label.TextProperty, "Total", stringFormat = "{0:C2}")

        grid.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        grid.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
        grid.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        grid.Children.Add(image, 0, 0)
        grid.Children.Add(date, 1, 0)
        grid.Children.Add(amount, 2, 0)
        self.View <- grid

type BasketListPage(goToAdd, goToBasket) as self =
    inherit ContentPage()
    
    let listView = new ListView(ItemTemplate = new DataTemplate(typeof<BasketViewCell>))
    let label    = new Label()
    let add      = new ToolbarItem("Add new basket", "basket_add", fun () -> goToAdd self.Navigation)

    let layout = 
        let layout = new StackLayout()
        layout.Children.Add(label)
        layout.Children.Add(listView)
        layout

    do
        listView.SetBinding(ListView.ItemsSourceProperty, "Baskets")
        listView.ItemTapped.Add(fun e -> goToBasket self.Navigation (unbox<Basket> e.Item))
        self.ToolbarItems.Add(add)
        
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

    let layout =
        let layout = new StackLayout()
        layout.Children.Add(entry)
        layout

    do
        self.SetBinding(ContentPage.TitleProperty, "Title")
        entry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "UpdateCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

        self.ToolbarItems.Add(save)
        self.Content <- layout


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

    let layout = 
        let layout = new StackLayout()
        layout.Children.Add(entry)
        layout

    do
        self.SetBinding(ContentPage.TitleProperty, "Title")
        entry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

        base.ToolbarItems.Add(save)
        base.Content <- layout


type StoreCell = {
    Id: int
    Name: string 
    GoToEdit: INavigation -> StoreCell -> unit
    RefreshStoreList: unit -> unit
}

type StoreViewCell() as self =
    inherit ViewCell()
    
    let grid    = new Grid()
    let name    = new Label(YAlign = TextAlignment.Center)
    let shop   = new Image(Source = FileImageSource.op_Implicit "shop")
    let edit    = new MenuItem(Text = "Edit", Icon = FileImageSource.op_Implicit "pencil")
    let delete  = new MenuItem(Text = "Delete", Icon = FileImageSource.op_Implicit "bin")

    do
        name.SetBinding(Label.TextProperty, "Name")
        edit.Clicked.Add(fun e -> self.Context.GoToEdit self.ParentView.Navigation self.Context)
        //crashes add command is on the view model not on the storecell
        //delete.SetBinding(MenuItem.CommandProperty, "ArchiveCommand")
        delete.Clicked.Add(fun _ -> self.Context.RefreshStoreList())
        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)

        grid.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        grid.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
        grid.Children.Add(shop, 0, 0)
        grid.Children.Add(name, 1, 0)
        self.View <- grid

    member self.Context
        with get(): StoreCell =
            unbox<StoreCell> self.BindingContext


type StoreListPage<'TStore>(getStoreList, goToAdd, goToEdit, goToBaskets) as self =
    inherit ContentPage()

    let listView = new ListView(ItemTemplate = new DataTemplate(typeof<StoreViewCell>))
    let add      = new ToolbarItem("Add new store", "shop_add", fun () -> goToAdd self.Navigation)
    
    do
        self.ToolbarItems.Add(add)
        self.Content <- listView
        listView.ItemTapped.Add(fun e -> goToBaskets self.Navigation <| unbox<'TStore> e.Item)

        self.SetBinding(ContentPage.TitleProperty, "Title")
            
    member self.Refresh() =
        listView.ItemsSource <- 
            getStoreList() 
            |> List.map(fun (s: Store) -> 
                { Id = s.Id
                  Name = s.Name
                  GoToEdit = goToEdit
                  RefreshStoreList = self.Refresh }: StoreCell)

    override self.OnAppearing() =
        self.Refresh()
        
type App() = 
    inherit Application()
    
    let addPage = 
        new AddStorePage()
    
    let updateStorePage = 
        new UpdateStorePage()

    let basketListPage = 
        new BasketListPage(
            goToAdd =
                (fun nav -> ()),
            goToBasket =
                (fun nav basket -> ()))

    let page = 
        new StoreListPage<StoreCell>(
            getStoreList = 
                Stores.list,
            goToAdd = 
                (fun nav ->
                    addPage.BindingContext <- new AddStoreViewModel("Add a new store", Stores.add)
                    nav.PushAsync(addPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToEdit =
                (fun nav store ->
                    updateStorePage.BindingContext <- new UpdateStoreViewModel("Update the store name", store.Name, Stores.update store.Id)
                    nav.PushAsync(updateStorePage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToBaskets = 
                (fun nav store ->
                    basketListPage.BindingContext <- new BasketListViewModel(store.Name, (fun () -> Baskets.list store.Id))
                    nav.PushAsync(basketListPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate)
        )

    do 
        page.BindingContext <- new PageViewModel(Title = "Stores")
        base.MainPage <- new NavigationPage(page)
namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open SQLite.Net
open Models

type BasketListPage() as self =
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
        with get(): StoreViewModel =
            unbox<StoreViewModel> self.BindingContext

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
            getStoreList() |> List.map(fun (s: Store) -> new StoreViewModel(s.Id, s.Name, goToEdit, self.Refresh))

    override self.OnAppearing() =
        self.Refresh()
        
type App() = 
    inherit Application()

    let addPage = new AddStorePage()
    let basketListPage = new BasketListPage()
    let updateStorePage = new UpdateStorePage()

    let vm = new PageViewModel(Title = "Stores")
    
    let page = 
        new StoreListPage<StoreViewModel>(
            Stores.list,
            (fun nav ->
                addPage.BindingContext <- new AddStoreViewModel(Stores.add)
                nav.PushAsync(addPage)
                |> Async.AwaitTask
                |> Async.StartImmediate),
            (fun nav store ->
                updateStorePage.BindingContext <- new UpdateStoreViewModel(store.Name, Stores.update store.Id)
                nav.PushAsync(updateStorePage)
                |> Async.AwaitTask
                |> Async.StartImmediate),
            (fun nav store ->
                basketListPage.BindingContext <- new PageViewModel(Title = store.Name)
                nav.PushAsync(basketListPage)
                |> Async.AwaitTask
                |> Async.StartImmediate)
        )

    do 
        page.BindingContext <- vm
        base.MainPage <- new NavigationPage(page)
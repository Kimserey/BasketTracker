namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Domain
open SQLite.Net

type StoreViewModel(storeId: int, connectionFactory: SQLiteConnectionFactory) =
    inherit ViewModelBase()

    let store =
        use conn = connectionFactory()
        Storage.Store.Get storeId conn

    let mutable name = store.Name

    member self.Name
        with get () = name
        and set value =
            self.OnPropertyChanging("Name") 
            name <- value
            self.OnPropertyChanged("Name")

    member self.Save() =
        use conn = connectionFactory() 
        Storage.Store.Update storeId name conn

type StorePage(storeId, connectionFactory: SQLiteConnectionFactory) =
    inherit ContentPage()
    
    let vm =
        new StoreViewModel(storeId, connectionFactory)

    do
        base.SetBinding(ContentPage.TitleProperty, "Name")
        base.BindingContext <- vm
    

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
        new MenuItem(Text = "Edit",  Icon = FileImageSource.op_Implicit "pencil")

    let delete =
        new MenuItem(Text = "Delete", Icon = FileImageSource.op_Implicit "bin")

    let layout =
        new StackLayout()
        |> StackLayout.AddChild (new Label() |> Label.SetBinding' Label.TextProperty "Name")

    do
        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)
        
        self.View <- layout

type StoresPage(connectionFactory: SQLiteConnectionFactory) as self =
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
    
    let refresh() =
        use conn = connectionFactory() 
        listView.ItemsSource <- Storage.Store.List conn
    
    do
        self.ToolbarItems.Add(add)
        self.Content <- listView
        
        listView.ItemTapped.Add(fun e ->
            let store = e.Item :?> Storage.Store
            self.Navigation.PushAsync(new StorePage(store.Id, connectionFactory))
            |> Async.AwaitTask
            |> Async.StartImmediate
            listView.SelectedItem <- null)

    override self.OnAppearing() =
        refresh()

type App() = 
    inherit Application()

    do 
        base.MainPage <- new NavigationPage(new StoresPage(Storage.connectionFactory))

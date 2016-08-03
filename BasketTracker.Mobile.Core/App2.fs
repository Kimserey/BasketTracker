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


type StoreMasterPage() as self =
    inherit ContentPage(Title = "test", Icon = FileImageSource.op_Implicit "hamburger")

    let title =
        let masterTitle =
            new Label(
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof<Label>),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            ) |> Label.SetBinding' Label.TextProperty "Name"

        new StackLayout(
            Padding = new Thickness(10.),
            Orientation = StackOrientation.Horizontal)
        |> StackLayout.AddChild (new Image(Source = FileImageSource.op_Implicit "shop"))
        |> StackLayout.AddChild masterTitle

    let buttonList =
        new ListView(ItemsSource = [ "Edit"; "Delete" ], ItemTemplate = new DataTemplate(typeof<TextCell>))
        |> ListView.SetTemplateBinding TextCell.TextProperty "."
        
    let layout =
        new StackLayout()
        |> StackLayout.AddChild title
        |> StackLayout.AddChild buttonList

    do
        base.SetBinding(ContentPage.TitleProperty, "Name")
        base.Content <- layout

type StorePage(storeId: int, connectionFactory: SQLiteConnectionFactory) as self =
    inherit MasterDetailPage(Title = "md test")

    let vm = 
        new StoreViewModel(storeId, connectionFactory)

    let detail =
        let page = new ContentPage(Title = "md test")
        page.SetBinding(ContentPage.TitleProperty, "Name")
        page.BindingContext <- vm
        page

    let master = 
        let page = new StoreMasterPage()
        page.BindingContext <- vm
        page

    let edit =
        new ToolbarItem(
            "Edit this store", 
            "pencil", 
            fun () -> self.IsPresented <- not self.IsPresented)

    do
        self.ToolbarItems.Add(edit)
        self.SetBinding(MasterDetailPage.TitleProperty, "Name")
        self.Detail <- detail
        self.Master <- master

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

type StoresPage(connectionFactory: SQLiteConnectionFactory) as self =
    inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit "shop")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<TextCell>))
        |> ListView.SetTemplateBinding TextCell.TextProperty "Name"
    
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
            |> Async.StartImmediate)

    override self.OnAppearing() =
        refresh()

type App() = 
    inherit Application()

    do 
        base.MainPage <- new NavigationPage(new StoresPage(Storage.connectionFactory))

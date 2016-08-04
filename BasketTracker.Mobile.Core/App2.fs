namespace BasketTracker.Mobile.Core

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
//    
//type EditStorePage(vm: StoreViewModel) as self =
//    inherit ContentPage(Title = "Edit existing store")
//
//    let nameEntry =
//        let entry = new Entry()
//        entry.SetBinding(Entry.TextProperty, "Name")
//        entry.Completed.Add(fun _ -> 
//            vm.Name <- entry.Text
//        )
//        entry
//
//    let save =
//        new ToolbarItem(
//            "Save your changes", 
//            "save", 
//            fun () -> 
//                vm.EditName()
//                self.Navigation.PopAsync()
//                |> Async.AwaitTask
//                |> Async.Ignore
//                |> Async.StartImmediate)
//
//    do
//        self.BindingContext <- vm
//        self.ToolbarItems.Add(save)
//        self.Content <- new StackLayout() |> StackLayout.AddChild nameEntry


type AddStorePage() as self =
    inherit ContentPage()
    
    let nameEntry =
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
        new StackLayout()
        |> StackLayout.AddChild nameEntry

    do
        self.SetBinding(ContentPage.TitleProperty, "Title")
        nameEntry.SetBinding(Entry.TextProperty, "Name")

        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

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
//        edit.Clicked.Add(fun e ->
//            let vm = self.BindingContext :?> StoreViewModel
//            let page = new EditStorePage(vm)
//            let host = vm.Host :?> StoresPage
//            host.Navigation.PushAsync(page)
//            |> Async.AwaitTask
//            |> Async.StartImmediate
//        )
//
        delete.SetBinding(MenuItem.CommandProperty, "ArchiveCommand")
        delete.Clicked.Add(fun e ->
            let vm = self.BindingContext :?> StoreSummaryViewModel
            let host = vm.Host :?> StoreListPage
            host.Refresh()
        )

        self.ContextActions.Add(edit)
        self.ContextActions.Add(delete)

        self.View <- layout

and StoreListPage(AddStorePage: Page) as self =
    inherit ContentPage(Title = "Stores")

    let listView = 
        new ListView(ItemTemplate = new DataTemplate(typeof<StoreViewCell>))
    
    let add =
        new ToolbarItem(
            "Add new store", 
            "plus", 
            fun () ->
                self.Navigation.PushAsync(AddStorePage)
                |> Async.AwaitTask
                |> Async.StartImmediate)
    
    do
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
       let vm = self.BindingContext :?> StoreListViewModel
       listView.ItemsSource <- (vm.List |> List.map (fun x -> x.Host <- self; x))

    override self.OnAppearing() =
        self.Refresh()

type App() = 
    inherit Application()

    do 
        let storeList = new StoreList(Storage.connectionFactory)
        let addPage = new AddStorePage()
        addPage.BindingContext <- new AddStoreViewModel(storeList.Add)
        let root = new StoreListPage(addPage)
        root.BindingContext <- new StoreListViewModel(storeList.List)
        
        base.MainPage <- new NavigationPage(root)

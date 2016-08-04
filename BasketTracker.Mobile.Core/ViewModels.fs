namespace BasketTracker.Mobile.Core

open System
open System.ComponentModel
open Xamarin.Forms
open Model

type ViewModelBase() =
    let propertyChanging = new Event<PropertyChangingEventHandler, PropertyChangingEventArgs>()
    let propertyChanged  = new Event<PropertyChangedEventHandler,  PropertyChangedEventArgs>()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member self.PropertyChanged = propertyChanged.Publish
    
    member self.PropertyChanging = propertyChanging.Publish

    member self.OnPropertyChanging name =
        propertyChanging.Trigger(self, new PropertyChangingEventArgs(name))

    member self.OnPropertyChanged name =
        propertyChanged.Trigger(self, new PropertyChangedEventArgs(name))

type PageViewModel() =
    inherit ViewModelBase()
    
    let mutable title = ""

    member self.Title 
        with get() = title
        and  set value = 
            self.OnPropertyChanging "Title"
            title <- value
            self.OnPropertyChanged "Title"

type StoreListViewModel() =
    inherit PageViewModel()

    let storeList = 
        new StoreList(Storage.connectionFactory)
    
    member self.NavigateToAddStoreViewModel() =
        new AddStoreViewModel(storeList.Add)

    member self.List
        with get() =
            storeList.List() |> List.map (fun (s: Store) -> new StoreCellViewModel(s))

and AddStoreViewModel(addStore) =
    inherit PageViewModel()

    let mutable name = ""

    do
        base.Title <- "Add a new store"
        
    member self.Name 
        with get() = name
        and  set value = 
            self.OnPropertyChanging "Name"
            name <- value
            self.OnPropertyChanged "Name"

    member self.AddCommand
        with get() =
            new Command<string>(fun name -> addStore name)

and UpdateStoreViewModel(store: Store) =
    inherit PageViewModel()

    let mutable name: string = store.Name

    do
        base.Title <- "Update store"
    
    member self.Name 
        with get() = name
        and  set value = 
            self.OnPropertyChanging "Name"
            name <- value
            self.OnPropertyChanged "Name"

    member self.UpdateCommand
        with get() =
            new Command<string>(fun name -> store.UpdateName name)


and StoreCellViewModel(store: Store) =
    inherit ViewModelBase()

    let mutable host: Page = Unchecked.defaultof<Page>

    member self.Host
        with get() = host
        and set value = host <- value

    member self.Name = 
        store.Name

    member self.GetUpdateViewModel() =
        new UpdateStoreViewModel(store)

    member self.NavigateToBaskListViewModel() =
        new BasketListViewModel(store)

    member self.ArchiveCommand
        with get() =
            new Command(fun () -> store.Archive())

and BasketListViewModel(store: Store) as self =
    inherit PageViewModel()

    do
        self.Title <- store.Name

    member self.List
        with get() =
            store.BasketList()

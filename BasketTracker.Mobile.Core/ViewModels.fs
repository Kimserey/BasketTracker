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


type StoreListViewModel(getList) =
    inherit PageViewModel()

    member self.List
        with get() =
            getList()
            |> List.map (fun (s: StoreSummary) -> 
                let archive() =
                    s.GetStore().Archive()

                new StoreSummaryViewModel(s.Name, archive))

and StoreSummaryViewModel(storeName, archive) =
    inherit ViewModelBase()

    let mutable host: Page = Unchecked.defaultof<Page>

    member self.Host
        with get() = host
        and set value = host <- value

    member self.Name = 
        storeName

    member self.ArchiveCommand
        with get() =
            new Command(fun () -> archive())

type AddStoreViewModel(addStore) =
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








//type StoreDetailViewModel() = 
//    inherit ViewModelBase()
//
//    let mutable title = ""
//    let mutable baskets: BasketViewModel list = []
//
//    member self.Title 
//        with get() = title
//        and  set value = 
//            self.OnPropertyChanging "Title"
//            title <- value
//            self.OnPropertyChanged "Title"
//
//    member self.Baskets
//        with get() = baskets
//        and set value =
//            self.OnPropertyChanging "Baskets"
//            baskets <- value
//            self.OnPropertyChanged "Baskets"
//        
//and BasketViewModel = {
//    Date: DateTime
//    Amount: Decimal
//    Items: BasketItem list
//} with
//    static member FromDomain (basket: Basket) =
//        let sum = 
//            basket.Items 
//            |> List.sumBy (fun i -> i.Amount)
//
//        { Date   = basket.Date
//          Amount = sum
//          Items = basket.Items }
//
//
//type BasketDetailViewModel() =
//    inherit ViewModelBase()
//
//    let mutable store = ""
//    let mutable date = DateTime.MinValue
//    let mutable items: BasketItem list = []
//
//    member self.Store
//        with get() = store
//        and set value =
//            self.OnPropertyChanging "Store"
//            store <- value
//            self.OnPropertyChanged "Store"
//    
//    member self.Date
//        with get() = date
//        and set value =
//            self.OnPropertyChanging "Date"
//            date <- value
//            self.OnPropertyChanged "Date"
//
//    member self.Sum
//        with get() =
//            items 
//            |> List.sumBy (fun item -> item.Amount)
//
//    member self.Items
//        with get() = items
//        and set value =
//            self.OnPropertyChanging "Items"
//            items <- value
//            self.OnPropertyChanged "Items"
//            self.OnPropertyChanged "Sum"
//
//    member self.AddItem item =
//        self.Items <- item::items
//
//    member self.RemoveItem item =
//        self.Items <- (items |> List.filter ((<>) item))

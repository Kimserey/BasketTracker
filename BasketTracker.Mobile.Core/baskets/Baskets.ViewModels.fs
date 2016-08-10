namespace BasketTracker.Mobile.Core.Baskets

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage
open Xamarin.Forms
open System
open System.Collections
open System.Collections.ObjectModel
open System.ComponentModel

type BasketListViewModel(storeId, storeName: string, api: BasketsApi) =
    inherit ListPageViewModel(Title = storeName)

    let mutable list = new ObservableCollection<BasketCellViewModel>()

    member self.List
        with get() = list
        and set value =
            base.OnPropertyChanging("List")
            list <- value
            base.OnPropertyChanged("List")

    member self.StoreId
        with get() = storeId

    override self.Refresh() = 
        let cells = 
            api.List storeId
            |> List.map (fun b -> new BasketCellViewModel(self, api, b))
            
        self.List <- new ObservableCollection<BasketCellViewModel>(cells)

and BasketCellViewModel(parent: BasketListViewModel, api: BasketsApi, basket: Basket) =
    inherit ViewModelBase()

    let mutable image = "basket"
    let mutable date = basket.Date
    let mutable total = basket.Total
    
    member self.Id
        with get() = basket.Id

    member self.Date
        with get() = date
        and set value =
            base.OnPropertyChanging "Date"
            date <- value
            base.OnPropertyChanged"Date"
    
    member self.Image
        with get() = image
        and set value =
            base.OnPropertyChanging "Image"
            image <- value
            base.OnPropertyChanged "Image"

    member self.Total
        with get() = total
        and set value =
            base.OnPropertyChanging "Total"
            total <- value
            base.OnPropertyChanged "Total"
            
    member self.RemoveCommand
        with get() =
            new Command(fun () ->
                api.Remove basket.Id
                parent.List.Remove self
                |> ignore)
    
type AddBasketViewModel(parent: BasketListViewModel, api: BasketsApi) =
    inherit PageViewModel(Title = "Add a new basket")

    let mutable date = DateTime.Now
    let mutable time = DateTime.Now.TimeOfDay

    member self.Date
        with get() = date
        and set value =
            base.OnPropertyChanging "Date"
            date <- value
            base.OnPropertyChanged "Date"

    member self.Time
        with get() = time
        and set value =
            base.OnPropertyChanging "Time"
            time <- value
            base.OnPropertyChanged "Time"
    
    member self.AddCommand
        with get() =
            new Command(fun () -> 
               let newBasket = api.Add parent.StoreId (self.Date.Add(self.Time))
               parent.List.Add (new BasketCellViewModel(parent, api, newBasket)))

type UpdateBasketViewModel(parent: BasketCellViewModel, api: BasketsApi) =
    inherit PageViewModel(Title = "Update basket")
    
    let mutable date = parent.Date.Date
    let mutable time = parent.Date.TimeOfDay

    member self.Date
        with get() = date
        and set value =
            base.OnPropertyChanging "Date"
            date <- value
            base.OnPropertyChanged "Date"

    member self.Time
        with get() = time
        and set value =
            base.OnPropertyChanging "Time"
            time <- value
            base.OnPropertyChanged "Time"

    member self.UpdateCommand
        with get() =
            new Command(fun () -> 
                let date = self.Date.Add(self.Time)
                api.Update parent.Id date
                parent.Date <- date)


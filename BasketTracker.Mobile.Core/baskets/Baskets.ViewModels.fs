namespace BasketTracker.Mobile.Core.Baskets

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage.Baskets
open Xamarin.Forms
open System
open System.Collections
open System.Collections.ObjectModel
open System.ComponentModel

module ViewModels =

    type BasketListViewModel(api: BasketsApi, store: Store) as self =
        inherit PageViewModel(Title = store.Name)

        let list =
            new ObservableCollection<BasketCellViewModel>(
                api.List store.Id
                |> List.map(fun b -> new BasketCellViewModel(self, api, b))
            )

        member self.List
            with get() = list

        member self.Store
            with get() = store

    and BasketCellViewModel(parent: BasketListViewModel, api: BasketsApi, basket: Basket) =
        inherit ViewModelBase()

        let mutable image = "basket"
        let mutable date = basket.Date
        let mutable total = basket.Total
        
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
        
    type AddBasketViewModel(parent: BasketListViewModel, api: BasketsApi) =
        inherit PageViewModel(Title = "Add a new basket to " + parent.Store.Name)

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
        
        member self.Add
            with get() =
                new Command<DateTime>(fun date -> 
                   let newBasket = api.Add parent.Store.Id date
                   parent.List.Add (new BasketCellViewModel(parent, api, newBasket)))


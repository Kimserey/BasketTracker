namespace BasketTracker.Mobile.Core.Items

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage
open Xamarin.Forms
open System
open System.Linq
open System.Collections
open System.Collections.ObjectModel
open System.ComponentModel

type ItemListViewModel(api:ItemsApi, basket: Basket) =
    inherit ListPageViewModel(Title = basket.Date.ToString("dd MMM yyyy"))

    let mutable list = 
        new ObservableCollection<ItemCellViewModel>()
    
    member self.List
        with get() = list
        and set value =
            base.OnPropertyChanging("List")
            list <- value
            base.OnPropertyChanged("List")

    member self.Basket
        with get() = basket

    override self.Refresh() = 
        let cells = 
            api.List basket.Id
            |> List.map(fun i -> new ItemCellViewModel(self, api, i))

        list <- new ObservableCollection<ItemCellViewModel>(cells)


and ItemCellViewModel(parent: ItemListViewModel, api:ItemsApi, item: Item) =
    inherit ViewModelBase()

    let mutable name = item.Name
    let mutable amount = item.Amount

    member self.Name
        with get() = name
        and set value =
            base.OnPropertyChanging "Name"
            name <- value
            base.OnPropertyChanged "Name"
            
    member self.Amount
        with get() = amount
        and set value =
            base.OnPropertyChanging "Amount"
            amount <- value
            base.OnPropertyChanged "Amount"

    member self.RemoveCommand
        with get() =
            new Command(fun () ->
                api.Remove item.Id
                parent.List.Remove self
                |> ignore)

type AddItemViewModel(parent: ItemListViewModel, api: ItemsApi)=
    inherit PageViewModel(Title = "Add a new item")

    let mutable name = ""
    let mutable amount = 0.m

    member self.Name
        with get() = name
        and set value =
            base.OnPropertyChanging "Name"
            name <- value
            base.OnPropertyChanged "Name"
            
    member self.Amount
        with get() = amount
        and set value =
            base.OnPropertyChanging "Amount"
            amount <- value
            base.OnPropertyChanged "Amount"

    member self.AddCommand
        with get() =
            new Command(fun () ->
                let item = api.Add parent.Basket.Id self.Name self.Amount
                parent.Refresh())
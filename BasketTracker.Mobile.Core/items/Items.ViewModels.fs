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

type ItemListViewModel(api:ItemsApi, basket: Basket) as self =
    inherit PageViewModel(Title = basket.Date.ToString("dd MMM yyyy"))

    let list =
        new ObservableCollection<ItemCellViewModel>(
            api.List basket.Id
            |> List.map(fun i -> new ItemCellViewModel(self, api, i))
        )

    member self.List
        with get() = list

    member self.Basket
        with get() = basket

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

    member self.AddCommant
        with get() =
            new Command(fun () ->
                let item = api.Add parent.Basket.Id self.Name self.Amount
                parent.List.Add(new ItemCellViewModel(parent, api, item)))
namespace BasketTracker.Mobile.Core.Stores

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel
open System.Collections.ObjectModel

type StoreListViewModel(api: StoresApi) =
    inherit ListPageViewModel(Title = "Stores")

    let mutable list = new ObservableCollection<StoreCellViewModel>()

    member self.List
        with get() = list
        and set value =
            base.OnPropertyChanging "List"
            list <- value
            base.OnPropertyChanged "List"

    override self.Refresh() = 
        let cells = 
            api.List() 
            |> List.map(fun (s: Store) -> new StoreCellViewModel(self, api, s))
        
        self.List <- new ObservableCollection<StoreCellViewModel>(cells)

and StoreCellViewModel(parentViewModel, api, store) =
    inherit ViewModelBase()

    let mutable name = store.Name

    member self.Store
        with get() = store

    member self.Name
        with get() = name
        and set value =
            base.OnPropertyChanging "Name"
            name <- value
            base.OnPropertyChanged "Name"

    member self.RemoveCommand
        with get() =
            new Command(fun () -> 
                api.Remove store.Id
                parentViewModel.List.Remove self
                |> ignore)


type AddStoreViewModel(parentViewModel: StoreListViewModel, api: StoresApi, title) =
    inherit PageViewModel(Title = title)

    let mutable name = ""

    member self.Name 
        with get() = name
        and  set value = 
            self.OnPropertyChanging "Name"
            name <- value
            self.OnPropertyChanged "Name"

    member self.AddCommand
        with get() = 
            new Command<string>(fun name ->
                let newStore = api.Add name
                parentViewModel.List.Add (new StoreCellViewModel(parentViewModel, api, newStore)))
                

type UpdateStoreViewModel(parentViewModel: StoreCellViewModel, api: StoresApi, title, store: Store) =
    inherit PageViewModel(Title = title)

    let mutable name: string = store.Name

    member self.Name 
        with get() = name
        and  set value = 
            self.OnPropertyChanging "Name"
            name <- value
            self.OnPropertyChanged "Name"

    member self.UpdateCommand
        with get() =
            new Command<string>(fun name -> 
                let updatedStore = api.Update store.Id name
                parentViewModel.Name <- name)

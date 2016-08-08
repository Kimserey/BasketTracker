namespace BasketTracker.Mobile.Core.Stores

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage.Stores
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel
open System.Collections.ObjectModel

module ViewModels =

    type StoreListViewModel(title, api: StoresApi) as self =
        inherit PageViewModel(Title = title)

        let list =
            new ObservableCollection<StoreCellViewModel>(
                api.List() |> List.map(fun (s: Store) -> new StoreCellViewModel(self, api, s))
            )
        
        member self.List
            with get() = list

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


    type AddStoreViewModel(parentViewModel: StoreListViewModel, api, title) =
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
                    

    type UpdateStoreViewModel(parentViewModel: StoreCellViewModel, api, title, store: Store) =
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

namespace BasketTracker.Mobile.Core.Stores

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel
open System.Collections.ObjectModel

module ViewModels =

    type StoreListViewModel(title, listStores, archiveStore) as self =
        inherit PageViewModel(Title = title)

        let list =
            new ObservableCollection<StoreCellViewModel>(
                listStores() 
                |> List.map(fun (s: Store) -> 
                    new StoreCellViewModel(
                        self,
                        s.Id, 
                        s.Name)))
        
        member self.List
            with get() = list

        member self.RemoveCommand
            with get() =
                new Command<StoreCellViewModel>(fun store -> 
                    archiveStore store.Id
                    self.List.Remove store 
                    |> ignore)

    and StoreCellViewModel(parentViewModel: StoreListViewModel, storeId, name) =
        inherit ViewModelBase()

        let mutable name = name
    
        member self.Id
            with get() = storeId

        member self.Name
            with get() = name
            and  set value = 
                self.OnPropertyChanging "Name"
                name <- value
                self.OnPropertyChanged "Name"

        member self.RemoveCommand
            with get() = parentViewModel.RemoveCommand

    
    type AddStoreViewModel(title, addStore) =
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
                    addStore name)

    type UpdateStoreViewModel(title, currentName, updateStoreName) =
        inherit PageViewModel(Title = title)

        let mutable name: string = currentName
    
        member self.Name 
            with get() = name
            and  set value = 
                self.OnPropertyChanging "Name"
                name <- value
                self.OnPropertyChanged "Name"

        member self.UpdateCommand
            with get() =
                new Command<string>(fun name -> updateStoreName name)
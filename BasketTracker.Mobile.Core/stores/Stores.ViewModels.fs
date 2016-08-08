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
                        s.Id, 
                        s.Name,
                        self.RemoveCommand)))
        
        member self.List
            with get() = list
                
        // CODE CRASH HERE
        // Breaks recursive loop
        member self.RemoveCommand
            with get() =
                new Command<StoreCellViewModel>(fun (store: StoreCellViewModel) -> 
                    archiveStore store.Id
                    self.List.Remove store
                    |> ignore)

    and StoreCellViewModel(storeId, name, removeCmd: Command<StoreCellViewModel>) =
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
            with get() = removeCmd

    
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
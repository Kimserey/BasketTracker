namespace BasketTracker.Mobile.Core

open System
open System.ComponentModel
open Xamarin.Forms
open Models

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


type BasketListViewModel(title, getList: unit -> Basket list) =
    inherit PageViewModel(Title = title)

    member self.List 
        with get() =
            getList()


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
            new Command<string>(fun name -> addStore name)


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


type StoreCellViewModel(archive) =
    inherit ViewModelBase()

    let mutable name = ""

    member self.Name
        with get() = name
        and  set value = 
            self.OnPropertyChanging "Name"
            name <- value
            self.OnPropertyChanged "Name"

    member self.ArchiveCommand
        with get() =
            new Command(fun () -> archive())
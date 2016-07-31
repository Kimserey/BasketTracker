namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Domain
open Xamarin.Forms

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


type StoreDetailViewModel() = 
    inherit ViewModelBase()

    let mutable title = ""
    let mutable baskets: BasketViewModel list = []

    member self.Title 
        with get() = title
        and  set value = 
            self.OnPropertyChanging "Title"
            title <- value
            self.OnPropertyChanged "Title"

    member self.Baskets
        with get() = baskets
        and set value =
            self.OnPropertyChanging "Baskets"
            baskets <- value
            self.OnPropertyChanged "Baskets"
        
and BasketViewModel = {
    Date: string
    Amount: string
} with
    static member FromDomain (basket: Basket) =
        let sum = 
            basket.Items 
            |> List.sumBy (fun i -> i.Amount)

        { Date   = basket.Date.ToString("dd MMM yyyy")
          Amount = sum.ToString("C2") }

type BasketDetailViewModel() =
    inherit ViewModelBase()

    let mutable title = ""
    let mutable items: BasketItemViewModel list = []

    member self.Title
        with get() = title
        and set value =
            self.OnPropertyChanging "Title"
            title <- value
            self.OnPropertyChanged "Title"

    member self.Items
        with get() = items
        and set value =
            self.OnPropertyChanging "Items"
            items <- value
            self.OnPropertyChanged "Items"

    member self.AddItem item =
        self.Items <- item::items

    member self.RemoveItem item =
        self.Items <- (items |> List.filter ((<>) item))

and BasketItemViewModel = {
    Key: int
    Title: string
    Amount: decimal
}

[<AutoOpen>]
module Store =

    //type BasketEntry(store) =
    //    inherit ContentPage(Title = store + " - basket")

    //type BasketCell() =
    //    inherit TextCell()

    //    do
    //        base.SetBinding(TextCell.TextProperty, "Date")
    //        base.SetBinding(TextCell.DetailProperty, "Amount")

    //type BasketList(baskets: BasketView list) =
    //    inherit ListView(ItemTemplate = new DataTemplate(typeof<BasketCell>))

    //    do
    //        base.BindingContext <- box { Items = baskets }
    //        base.SetBinding(ListView.ItemsSourceProperty, "Items")


    //type StoreBasketListPage(defaultStore: Store) =
    //    inherit ContentPage(Title = defaultStore.Name)

    //    do
    //        base.Content <- new BasketList(

    //type StoreDetailPage(defaultStore: Store) as self =
    //    inherit NavigationPage()

    //    let item =
    //        new ToolbarItem(
    //            "New",
    //            "plus",
    //            fun () ->
    //                self.PushAsync(new BasketEntry(defaultStore.Name))
    //                |> Async.AwaitTask
    //                |> Async.StartImmediate)

    //    do
    //        base.ToolbarItems.Add(item)

    //    override self.OnBindingContextChanged() =
    //        let store = self.BindingContext :?> Store
    //        base.CurrentPage.BindingContext <- store

    type BasketDetailPage(vm: BasketDetailViewModel) =
        inherit ContentPage()

        do
            base.BindingContext <- vm
            base.SetBinding(ContentPage.TitleProperty, "Title")

    type StoreDetailPage(vm: StoreDetailViewModel) as self =
        inherit ContentPage()

        let basketList = 
            new ListView(ItemTemplate = new DataTemplate(typeof<TextCell>))
        
        do
            base.BindingContext <- vm
            base.SetBinding(ContentPage.TitleProperty, "Title")
            base.Content <- basketList

            basketList
                .ItemTemplate
                .SetBinding(TextCell.TextProperty, "Date")
            basketList
                .ItemTemplate
                .SetBinding(TextCell.DetailProperty, "Amount")
            basketList
                .SetBinding(ListView.ItemsSourceProperty, "Baskets")
            basketList
                .ItemSelected
                .Add(fun e -> 
                    let selection = e.SelectedItem :?> BasketViewModel

                    let basketDetail = 
                        new BasketDetailPage(new BasketDetailViewModel(Title = sprintf "%s - %s" selection.Date selection.Amount))

                    self.Navigation.PushAsync(basketDetail)
                    |> Async.AwaitTask
                    |> Async.StartImmediate)


    type StoreMasterPage(stores, onSelect) as self =
        inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit "hamburger")

        let (Stores stores) = 
            stores

        let title =
            let layout = 
                new StackLayout(
                    Padding = new Thickness(10.),
                    Orientation = StackOrientation.Horizontal)
            
            layout
                .Children
                .Add(new Image(Source = FileImageSource.op_Implicit "shop_black"))

            layout
                .Children
                .Add(
                    new Label(
                        Text = "Stores",
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof<Label>),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand))
            layout

        let menu = 
            let list =
                new ListView(
                    ItemsSource = stores,
                    ItemTemplate = new DataTemplate(typeof<TextCell>),
                    VerticalOptions = LayoutOptions.FillAndExpand)

            list
                .ItemSelected
                .Add(fun e -> onSelect(self, e.SelectedItem :?> Store))
            list
                .ItemTemplate
                .SetBinding(TextCell.TextProperty, "Name")
            list
        
        let layout =
            new StackLayout()

        do
            layout
                .Children
                .Add(title)

            layout
                .Children
                .Add(menu)

            base.Content <- layout

    type Root() as self =
        inherit MasterDetailPage()

        let stores =
            Stores.Sample

        do
            let vm = new StoreDetailViewModel()
            self.Detail <- 
                new NavigationPage(StoreDetailPage(vm))

            self.Master <- 
                new StoreMasterPage(stores, (fun (page, selection) -> 
                    vm.Title <- selection.Name
                    vm.Baskets <- selection.Baskets |> List.map BasketViewModel.FromDomain
                    self.IsPresented <- false))
             
type App() = 
    inherit Application()

    do 
        base.MainPage <- new Root()

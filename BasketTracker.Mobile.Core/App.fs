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
    Date: DateTime
    Amount: Decimal
    Items: BasketItem list
} with
    static member FromDomain (basket: Basket) =
        let sum = 
            basket.Items 
            |> List.sumBy (fun i -> i.Amount)

        { Date   = basket.Date
          Amount = sum
          Items = basket.Items }

type BasketDetailViewModel() =
    inherit ViewModelBase()

    let mutable store = ""
    let mutable date = DateTime.MinValue
    let mutable items: BasketItem list = []

    member self.Store
        with get() = store
        and set value =
            self.OnPropertyChanging "Store"
            store <- value
            self.OnPropertyChanged "Store"
    
    member self.Date
        with get() = date
        and set value =
            self.OnPropertyChanging "Date"
            date <- value
            self.OnPropertyChanged "Date"

    member self.Sum
        with get() =
            items 
            |> List.sumBy (fun item -> item.Amount)

    member self.Items
        with get() = items
        and set value =
            self.OnPropertyChanging "Items"
            items <- value
            self.OnPropertyChanged "Items"
            self.OnPropertyChanged "Sum"

    member self.AddItem item =
        self.Items <- item::items

    member self.RemoveItem item =
        self.Items <- (items |> List.filter ((<>) item))

[<AutoOpen>]
module Extensions =

    type Label with
        static member SetBinding' prop (name: string) (label: Label) =
            label.SetBinding(prop, name)
            label

    type ListView with
        static member SetTemplateBinding prop (name: string) (list: ListView) =
            list.ItemTemplate.SetBinding(prop, name)
            list

    type StackLayout with
        static member AddChild child (layout: StackLayout) =
            layout.Children.Add child
            layout

[<AutoOpen>]
module Store =

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
        inherit ContentPage(Title = "Basket")

        let itemList = 
            new ListView(ItemsSource = vm.Items, ItemTemplate = new DataTemplate(typeof<TextCell>))
            |> ListView.SetTemplateBinding TextCell.TextProperty "Name"
            |> ListView.SetTemplateBinding TextCell.DetailProperty "Amount"

        let header =
            new StackLayout(Padding = new Thickness(10.))
            |> StackLayout.AddChild (new Label(Text = vm.Store))
            |> StackLayout.AddChild (new Label(Text = vm.Date.ToString("dd MMM yyyy")))
            |> StackLayout.AddChild (new Label() |> Label.SetBinding' Label.TextProperty "Sum")

        let pageLayout =
            new StackLayout()
            |> StackLayout.AddChild header
            |> StackLayout.AddChild itemList
        do
            base.BindingContext <- vm
            base.Content <- pageLayout

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

                    let basketVM =
                        new BasketDetailViewModel(
                            Store = vm.Title,
                            Date = selection.Date,
                            Items = selection.Items)

                    let basketDetail = 
                        new BasketDetailPage(basketVM)

                    self.Navigation.PushAsync(basketDetail)
                    |> Async.AwaitTask
                    |> Async.StartImmediate)


    type StoreMasterPage(stores, onSelect) as self =
        inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit "hamburger")

        let (Stores stores) = 
            stores

        let title =
            new StackLayout(
                Padding = new Thickness(10.),
                Orientation = StackOrientation.Horizontal)
            |> StackLayout.AddChild (new Image(Source = FileImageSource.op_Implicit "shop_black"))
            |> StackLayout.AddChild 
                    (new Label(
                        Text = "Stores",
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof<Label>),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand))

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
        
        let defaultStore =
            let (Stores stores) = stores
            stores.[0]

        let vm = 
            new StoreDetailViewModel(Title = defaultStore.Name, Baskets = (defaultStore.Baskets |> List.map BasketViewModel.FromDomain))
        do
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

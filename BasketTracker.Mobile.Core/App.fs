namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open Domain

type ItemViewCell() =
    inherit ViewCell()

    let title =
        new Label()
        |> Label.SetBinding' Label.TextProperty "Name"
    
    let amount =
        new Label()
        |> Label.SetBinding' Label.TextProperty "Amount"

    let layout =
        new StackLayout(Orientation = StackOrientation.Horizontal)
        |> StackLayout.AddChild title
        |> StackLayout.AddChild amount

    let menuItem =
        new MenuItem(Text = "Remove", Icon = FileImageSource.op_Implicit "bin", IsDestructive = true)
        |> MenuItem.SetBinding' MenuItem.CommandProperty "Delete"

    do
        base.View <- layout
        base.ContextActions.Add(menuItem)

type BasketDetailPage(vm: BasketDetailViewModel) =
    inherit ContentPage(Title = "Basket")

    let itemList = 
        new ListView(ItemsSource = vm.Items, ItemTemplate = new DataTemplate(typeof<ItemViewCell>))
    
    let header =
        new StackLayout()
        |> StackLayout.AddChild (new Label(Text = vm.Store))
        |> StackLayout.AddChild (new Label(Text = vm.Date.ToString("dd MMM yyyy")))
        |> StackLayout.AddChild (new Label() |> Label.SetBinding' Label.TextProperty "Sum")

    let pageLayout =
        new StackLayout(Padding = new Thickness(10.))
        |> StackLayout.AddChild header
        |> StackLayout.AddChild itemList
    
    let toolBarItem =
        new ToolbarItem(
            "Add new item",
            "plus",
            fun () -> ())

    do
        base.ToolbarItems.Add(toolBarItem)
        base.BindingContext <- vm
        base.Content <- pageLayout

type StoreDetailPage(vm: StoreDetailViewModel) as self =
    inherit ContentPage()

    let basketList = 
        new ListView(ItemTemplate = new DataTemplate(typeof<TextCell>))
        |> ListView.SetTemplateBinding TextCell.TextProperty "Date"
        |> ListView.SetTemplateBinding TextCell.DetailProperty "Amount"
        |> ListView.SetBinding' ListView.ItemsSourceProperty "Baskets"

    do
        base.BindingContext <- vm
        base.SetBinding(ContentPage.TitleProperty, "Title")
        base.Content <- basketList

        basketList
            .ItemSelected
            .Add(fun e -> 
                let selection = e.SelectedItem :?> BasketViewModel

                let basketVM =
                    new BasketDetailViewModel(
                        Store = vm.Title,
                        Date = selection.Date,
                        Items = selection.Items
                    )
            
                let basketDetail = 
                    new BasketDetailPage(basketVM)

                self.Navigation.PushAsync(basketDetail)
                |> Async.AwaitTask
                |> Async.StartImmediate)


type StoreMasterPage'(stores, onSelect) as self =
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
            .Add(fun e -> 
                onSelect(self, e.SelectedItem :?> Store))
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
            new StoreMasterPage'(stores, (fun (page, selection) -> 
                vm.Title <- selection.Name
                vm.Baskets <- selection.Baskets |> List.map BasketViewModel.FromDomain
                self.IsPresented <- false))
             
type App'() = 
    inherit Application()

    do 
        base.MainPage <- new Root()

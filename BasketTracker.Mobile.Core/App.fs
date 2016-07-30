namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open Domain
open Xamarin.Forms

type BasketView = {
        Date: string
        Amount: string
    } with
        static member FromDomain (basket: Basket) =
            { Date = basket.Date.ToString("dd MMM yyyy")
              Amount = (basket.Items |> List.sumBy (fun i -> i.Amount)).ToString("C2") }

[<AutoOpen>]
module Store =

    type BasketCell() =
        inherit TextCell()

        do
            base.SetBinding(TextCell.TextProperty, "Date")
            base.SetBinding(TextCell.DetailProperty, "Amount")

    type BasketList(baskets: BasketView list) =
        inherit ListView(ItemsSource = baskets, ItemTemplate = new DataTemplate(typeof<BasketCell>))

    type StoreDetailPage(name, baskets) as self =
        inherit NavigationPage(new ContentPage(Title = name, Content = BasketList(baskets)))

        let item =
            new ToolbarItem(
                "New",
                "plus",
                fun () ->
                    self.DisplayAlert("Alert", "Something", "OK") 
                    |> Async.AwaitTask 
                    |> Async.Start)

        do
            base.ToolbarItems.Add(item)

    type StoreMenuItemCell() =
        inherit TextCell()

        do
            base.SetBinding(TextCell.TextProperty, "Name")

    type StoreMasterPage(stores) =
        inherit ContentPage(Title = "Stores", Icon = FileImageSource.op_Implicit"hamburger")

        let (Stores stores) = stores

        let title =
            let layout = 
                new StackLayout(
                    Padding = new Thickness(10.),
                    Orientation = StackOrientation.Horizontal)
            
            layout
                .Children
                .Add(new Image(Source = FileImageSource.op_Implicit "shop"))

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
            new ListView(
                ItemsSource = stores,
                ItemTemplate = new DataTemplate(typeof<StoreMenuItemCell>),
                VerticalOptions = LayoutOptions.FillAndExpand)
        
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

    type Root() =
        inherit MasterDetailPage()

        let stores =
            Stores.Sample

        let store = 
            let (Stores stores) = stores
            stores.[0]

        let baskets =
            store.Baskets
            |> List.map BasketView.FromDomain

        do
            base.Master <- new StoreMasterPage(stores)
            base.Detail <- new StoreDetailPage(store.Name, baskets)
         
type App() = 
    inherit Application()

    do 
        base.MainPage <- new Root()

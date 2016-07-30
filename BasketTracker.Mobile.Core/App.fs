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

    type StorePage(name, baskets) as self =
        inherit NavigationPage(new ContentPage(Title = name, Content = BasketList(baskets)))

        do
            let item =
                new ToolbarItem(Text = "New", Icon = FileImageSource.op_Implicit "plus.png")

            item.Clicked.Add(fun e ->
                self.DisplayAlert("Alert", "Something", "OK") 
                |> Async.AwaitTask 
                |> Async.Start)

            base.ToolbarItems.Add(item)

    type StoreMasterDetailPage() =
        inherit MasterDetailPage()

        do
            base.Master <- new ContentPage()
            base.Detail <- new ContentPage()
         
type App() = 
    inherit Application()

    do 
        let store = 
            let (Stores stores) = Stores.Sample
            stores.[0]

        let baskets =
            store.Baskets
            |> List.map BasketView.FromDomain

        base.MainPage <- new StorePage(store.Name, baskets)

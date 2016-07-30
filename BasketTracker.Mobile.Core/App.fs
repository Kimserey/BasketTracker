namespace BasketTracker.Mobile.Core

open System
open Domain
open Xamarin.Forms

type BasketView = {
        Date: DateTime
        Amount: string
    }

[<AutoOpen>]
module Store =

    type BasketCell() =
        inherit TextCell()

        do
            base.SetBinding(TextCell.TextProperty, "Date")
            base.SetBinding(TextCell.DetailProperty, "Amount")

    type BasketList(baskets: BasketView list) =
        inherit ListView(ItemsSource = baskets, ItemTemplate = new DataTemplate(typeof<BasketCell>))

    type StorePage(name, baskets) =
        inherit ContentPage(Title = name, Content = BasketList(baskets))

type App() = 
    inherit Application()

    do 
        let store = 
            let (Stores stores) = Stores.Sample
            stores.[0]

        let baskets =
            store.Baskets
            |> List.map(fun b -> 
                { Date = b.Date
                  Amount = (b.Items |> List.sumBy (fun i -> i.Amount)).ToString("C2") })
                
        base.MainPage <- new StorePage(store.Name, baskets)

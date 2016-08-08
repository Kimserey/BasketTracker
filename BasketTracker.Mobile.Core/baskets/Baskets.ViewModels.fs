namespace BasketTracker.Mobile.Core.Baskets

open BasketTracker.Mobile.Core
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage.Baskets
open Xamarin.Forms
open System
open System.Collections
open System.Collections.ObjectModel
open System.ComponentModel

module ViewModels =

    type BasketListViewModel() =
        
        let list =
            new ObservableCollection<Basket>([])

        member self.List
            with get() = list

    and BasketViewCell(navigator: Navigator) as self=
        inherit ViewCell()

        let image   = new Image(Source = FileImageSource.op_Implicit "basket")
        let date    = new Label(YAlign = TextAlignment.Center)
        let amount  = new Label(YAlign = TextAlignment.Center)

        let layout = 
            let layout = new Grid()
            layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
            layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
            layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
            layout.Children.Add(image, 0, 0)
            layout.Children.Add(date, 1, 0)
            layout.Children.Add(amount, 2, 0)
            layout

        do
            // Navigation events
            self.Tapped.Add(fun _ -> navigator.Basket.NavigateToBasketList navigator <| Context self.BindingContext)

            // Bindings
            date.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:dd MMM yyyy - hh:mm tt}")
            amount.SetBinding(Label.TextProperty, "Total", stringFormat = "{0:C2}")

            self.View <- layout

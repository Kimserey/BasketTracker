namespace BasketTracker.Mobile.Core.Baskets

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel

type BasketListPage(vm, navigator: Navigator) as self =
    inherit ContentPage()

    let listView = new ListView(ItemTemplate = new DataTemplate(fun () -> box (new BasketViewCell(navigator))))
    let label    = new Label()
    let add = 
        new ToolbarItem(
            "Add new basket", 
            "basket_add", 
            fun () -> navigator.Basket.NavigateToAdd navigator <| Context self.BindingContext)

    let layout = 
        let layout = new StackLayout()
        layout.Children.Add(label)
        layout.Children.Add(listView)
        layout

    do
        // Bindings
        listView.SetBinding(ListView.ItemsSourceProperty, "List")
        self.SetBinding(ContentPage.TitleProperty, "Title")

        // Toolbar items
        self.ToolbarItems.Add(add)
        
        self.BindingContext <- vm
        self.Content <- layout

and BasketViewCell(navigator: Navigator) as self=
    inherit ViewCell()

    let image  = new Image()
    let date   = new Label(YAlign = TextAlignment.Center)
    let amount = new Label(YAlign = TextAlignment.Center)

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
        self.Tapped.Add(fun _ -> navigator.Item.NavigateToItemList navigator <| Context self.BindingContext)

        // Bindings
        image.SetBinding(Image.SourceProperty, "Image")
        date.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:dd MMM yyyy - hh:mm tt}")
        amount.SetBinding(Label.TextProperty, "Total", stringFormat = "{0:C2}")

        self.View <- layout

type AddBasketPage(vm, navigator: Navigator) as self =
    inherit ContentPage()

    let date = new DatePicker()
    let time = new TimePicker()
    let save =
        new ToolbarItem(
            "Save this basket", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout()
        layout.Children.Add(date)
        layout.Children.Add(time)
        layout

    do
        date.SetBinding(DatePicker.DateProperty, "Date")            
        time.SetBinding(TimePicker.TimeProperty, "Time")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")

        // Toolbar items
        base.ToolbarItems.Add(save)

        base.BindingContext <- vm
        base.Content <- layout
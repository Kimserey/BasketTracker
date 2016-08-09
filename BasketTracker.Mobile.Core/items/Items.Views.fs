namespace BasketTracker.Mobile.Core.Items

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel

type ItemListPage(vm, navigator: Navigator) as self =
    inherit ContentPage()

    let listView = new ListView(ItemTemplate = new DataTemplate(fun () -> box (new ItemViewCell(navigator))))
    let label    = new Label()
    let add = 
        new ToolbarItem(
            "Add new item", 
            "plus", 
            fun () -> navigator.Item.NavigateToAdd navigator <| Context self.BindingContext)

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

and ItemViewCell(navigator: Navigator) as self =
    inherit ViewCell()

    let name   = new Label(XAlign = TextAlignment.Start)
    let amount = new Label(XAlign = TextAlignment.End)

    let layout = 
        let layout = new Grid()
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        layout.Children.Add(name, 0, 0)
        layout.Children.Add(amount, 1, 0)
        layout

    do
        // Navigation events
        self.Tapped.Add(fun _ -> navigator.Item.NavigateToItemList navigator <| Context self.BindingContext)

        // Bindings
        name.SetBinding(Label.TextProperty, "Name")
        amount.SetBinding(Label.TextProperty, "Amount", stringFormat = "{0:C2}")

        self.View <- layout

type AddItemPage(vm, navigator: Navigator) as self =
    inherit ContentPage()

    do
        self.SetBinding(ContentPage.TitleProperty, "Title") 
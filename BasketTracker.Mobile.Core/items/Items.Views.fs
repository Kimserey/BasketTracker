namespace BasketTracker.Mobile.Core.Items

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel

type ItemListPage(vm: ListPageViewModel, navigator: Navigator) as self =
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

    override self.OnAppearing() =
        base.OnAppearing()
        vm.Refresh()

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
        // Bindings
        name.SetBinding(Label.TextProperty, "Name")
        amount.SetBinding(Label.TextProperty, "Amount", stringFormat = "{0:C2}")

        self.View <- layout

type AddItemPage(vm) as self =
    inherit ContentPage()

    let name = new Entry()
    let amount = new Entry(Keyboard = Keyboard.Numeric)
    
    let save =
        new ToolbarItem(
            "Save this item", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout()
        layout.Children.Add(name)
        layout.Children.Add(amount)
        layout

    do
        // Bindings
        name.SetBinding(Entry.TextProperty, "Name")
        amount.SetBinding(Entry.TextProperty, "Amount")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
        
        // Toolbar items
        base.ToolbarItems.Add(save)

        self.BindingContext <- vm
        self.Content <- layout